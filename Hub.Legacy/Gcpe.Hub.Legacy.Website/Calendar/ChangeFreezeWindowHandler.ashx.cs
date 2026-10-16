using CorporateCalendar.Security;
using System;
using System.Web;

namespace Gcpe.Hub.Calendar
{
    public class ChangeFreezeWindowHandler : IHttpHandler
    {
        private CustomPrincipal _customPrincipal = null;
        public CustomPrincipal CustomPrincipal
        {
            get
            {
                if (_customPrincipal == null)
                {
                    _customPrincipal = new CustomPrincipal(HttpContext.Current.User.Identity);
                }
                return _customPrincipal;
            }
        }

        bool? isHq = null;
        private bool IsHq
        {
            get
            {
                if (!isHq.HasValue)
                    isHq = CustomPrincipal.IsGCPEHQ && CustomPrincipal.IsInRoleOrGreater(SecurityRole.Editor);
                return isHq.Value;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            var rvl = false;

            rvl = Gcpe.Time.ChangeFreezeWindow.IsFrozen(DateTimeOffset.UtcNow, IsHq, Gcpe.Time.BcTime.Default);

            context.Response.ContentType = "text/plain";
            context.Response.Write(rvl);
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