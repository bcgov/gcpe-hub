extern alias legacy;

using Gcpe.Hub.News.ReleaseManagement;
using Gcpe.News.ReleaseManagement;
using System;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.Calendar
{
    /// <summary>
    /// Summary description for ReleaseDownloadHandler
    /// </summary>
    public class ReleaseDownloadHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var id = context.Request?.QueryString?["releaseId"];
            var customPrincipal = new CorporateCalendar.Security.CustomPrincipal(HttpContext.Current.User.Identity);

            var releaseId = Guid.Parse(id);
            using (var hub = new legacy::Gcpe.Hub.Data.Entity.HubEntities())
            {
                var release = hub.NewsReleases.SingleOrDefault(r => r.Id == releaseId);

                //Redirect the user to the Hub if they have access, otherwise return the PDF document.
                if(hub.Users.Any(u => u.EmailAddress == customPrincipal.Email))
                {
                    context.Response.Redirect(ReleaseModel.ReleaseHubUrl(release), false);
                }
                else
                {
                    try
                    {
                        var releaseTemplate = Gcpe.News.ReleaseManagement.Templates.Release.FromEntity(release);
                        byte[] pdfData = releaseTemplate.ToPortableDocument();

                        context.Response.ContentType = "application/pdf";
                        context.Response.AddHeader("Content-Disposition", "inline; filename=New-" + DateTime.UtcNow.Ticks + ".pdf");
                        context.Response.BinaryWrite(pdfData);
                    }
                    catch (Exception e)
                    {
                        Utils.LogError("An error occured redirecting to, or downloading a release document.", e);
                    }
                }
            }

            context.ApplicationInstance.CompleteRequest();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}