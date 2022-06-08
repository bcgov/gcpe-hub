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

            if (!IsHq && IsDailyChangeFreeze()) rvl = true;

            context.Response.ContentType = "text/plain";
            context.Response.Write(rvl);
        }

        private bool IsDailyChangeFreeze()
        {
            var pacificNow = GetCurrentPacificTime();
            TimeSpan start = new TimeSpan(4, 05, 0); // 4:05 PM
            TimeSpan end = new TimeSpan(17, 0, 0); // 5 PM
            TimeSpan now = pacificNow.TimeOfDay;
            var withinFreezeWindow = (now > start) && (now < end);

            return withinFreezeWindow;
        }

        private DateTime GetCurrentPacificTime()
        {
            var pacificTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            return TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow.DateTime, TimeZoneInfo.Utc, pacificTimeZone);
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