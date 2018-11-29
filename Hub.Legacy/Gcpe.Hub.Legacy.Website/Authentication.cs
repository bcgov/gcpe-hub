using System.Web;
using System.Security.Principal;
using CorporateCalendar.Security;
using Gcpe.Hub.Configuration;

namespace Gcpe.Hub
{
    public static class Authentication
    {
        public static bool NeedsNewsValidation(string remote_addr, string rawUrl)
        {
            return ((!System.Diagnostics.Debugger.IsAttached || remote_addr != "::1")
                && rawUrl.StartsWith("/Legacy/News", System.StringComparison.OrdinalIgnoreCase));
        }

        public static void GlobalAuthenticateRequest(HttpRequest Request, HttpResponse Response)
        {
            string userName = string.Empty;

            string remote_addr = Request.ServerVariables["REMOTE_ADDR"];
            if (!NeedsNewsValidation(remote_addr, Request.RequestContext.HttpContext.Request.RawUrl)) return;

            if (App.Settings.TrustedReverseProxyServers.ToCollection().Contains(remote_addr))
            {
                userName = Request.Headers["SM_USER"];
            }
            else
            {
                string logon_user = Request.ServerVariables["LOGON_USER"];
                if (string.IsNullOrWhiteSpace(logon_user))
                {
                    RequestWindowsAuthentication(Response); // return 401-Unauthorized to trigger a Windows Authentication Form
                    return;
                }
                userName = logon_user;
            }
        }

        public static void GlobalAuthorizeRequest(HttpRequest Request, HttpResponse Response)
        {
            string rawUrl = Request.RequestContext.HttpContext.Request.RawUrl;
            bool accessFromRoot = rawUrl == "/" || rawUrl == "/Legacy/";
            if (!accessFromRoot && !NeedsNewsValidation(Request.ServerVariables["REMOTE_ADDR"], rawUrl)) return;

            //Issue: SiteMinder does not authenticate when serving image files
            //if (Properties.Settings.Default.TrustedReverseProxyServers.Contains(remote_addr))
            //{
            //    if (Request.RequestContext.HttpContext.Request.RawUrl.EndsWith(".png") || Request.RequestContext.HttpContext.Request.RawUrl.EndsWith(".gif") || Request.RequestContext.HttpContext.Request.RawUrl.EndsWith(".ico"))
            //        return;
            //}

            IIdentity userIdentity = Request.RequestContext.HttpContext.User.Identity;
            if (!string.IsNullOrEmpty(userIdentity.Name) && userIdentity.IsAuthenticated)
            {
                bool? isHubUser = Request.RequestContext.HttpContext.Cache[userIdentity.Name + "_IsHubUser"] as bool?;
                if (!isHubUser.HasValue)
                {
                    var customPrincipal = new CustomPrincipal(userIdentity);

                    if (!customPrincipal.IsAuthorized)
                    {
                        var log = new CorporateCalendar.Logging.Log(
                            string.Format("User ({0}) attempted to access a resource or asset but is not authorized.", customPrincipal.Username),
                                CorporateCalendar.Logging.Log.LogType.Warning);
                        throw new CorporateCalendar.Exception.NotAuthorizedException(
                            string.Format("User ({0}) is not authorized to view this page.",
                                customPrincipal.Username));
                    }
                    isHubUser = customPrincipal.IsInRoleOrGreater(SecurityRole.Advanced);
                    Request.RequestContext.HttpContext.Cache[userIdentity.Name + "_IsHubUser"] = isHubUser;
                }

                if (accessFromRoot)
                {
                    Response.Redirect(isHubUser.Value ? "~/News/ReleaseManagement/Drafts" : "~/Calendar");
                    return;
                }
                if (isHubUser.Value)
                    return;
            }

            RequestWindowsAuthentication(Response);
            //throw new HttpException(403, null);
        }

        /// <summary>
        /// this is the earliest event where the session is available
        /// since the classes to obtain the group and user information store info in session,
        /// this is the earliest I can execute them
        /// putting them earlier would result in making the calls to the AD server on every page load, impacting performance
        ///
        /// this method detects whether the request comes from the whitelist (SiteMinder proxy) or otherwise.
        /// if from the whitelist, the SM_USER header is used as it will have been set by SiteMinder. Otherwise,
        /// the user will have passed through NTLM auth and it will use the LOGON_USER server variable
        /// 
        /// it then constructs a custom principal object from the logged in user's id, which fetches the user's
        /// information (display name, guid, email) and attaches the user's AD groups as "roles" and sets this user
        /// as the user for the current context
        /// </summary>
        /// <param name="Context"></param>
        /// <param name="Request"></param>
        public static void GlobalAcquireRequestState(HttpContext Context, HttpRequest Request)
        {
            //string userName;

            //if (System.Diagnostics.Debugger.IsAttached && Request.ServerVariables["REMOTE_ADDR"] == "::1")
            //{
            //    userName = Environment.UserName;
            //}
            //else
            //{
            //    if (Properties.Settings.Default.TrustedReverseProxyServers.Contains(Request.ServerVariables["REMOTE_ADDR"]))
            //        userName = Request.Headers["SM_USER"];
            //    else
            //        userName = Request.ServerVariables["LOGON_USER"];
            //}

            //if (!string.IsNullOrEmpty(userName))
            //    Context.User = new GenericPrincipal(new GenericIdentity(userName), null);

            //
        }

        /// <summary>
        /// Convenience method to send a 401 response
        /// </summary>
        private static void RequestWindowsAuthentication(HttpResponse Response)
        {
            // Create a 401 response, the browser will show the log-in dialogbox, asking the user to supply new credentials, 
            // if browser is not set to "automatically sign in with current credentials"
            // sign in with current credentials depends on url being in trusted intranet
            Response.Buffer = true;
            Response.StatusCode = 401;
            Response.StatusDescription = "Unauthorized";

            // A authentication header must be supplied. This header can be changed to Negotiate when using Kerberos authentication
            Response.AddHeader("WWW-Authenticate", "NTLM");

            // Send the 401 response
            Response.End();
        }
    }
}