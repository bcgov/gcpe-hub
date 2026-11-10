using CorporateCalendar.Data;
using CorporateCalendar.Security;
using System;
using System.Linq;
using System.Web;
using Log = CorporateCalendar.Data.Log;

namespace Gcpe.Hub.Calendar
{
    /// <summary>
    /// Summary description for ActivityEditingHandler
    /// </summary>
    public class ActivityEditingHandler : IHttpHandler
    {

        HttpRequest request = null;
        private readonly CorporateCalendarDataContext _db;

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

        public ActivityEditingHandler()
        {
            _db = new CorporateCalendarDataContext();
        }

        public void ProcessRequest(HttpContext context)
        {
            request = context.Request;
            string operation = request.QueryString["Op"];

            switch (operation)
            {
                case "GetActivityStatus":
                    GetActivityStatus(context);
                    break;
                case "CancelEditingActivity":
                    CancelEditingActivity(context);
                    break;
            }
        }

        private void CancelEditingActivity(HttpContext context)
        {
            string id = context.Request["activityId"];
            if (string.IsNullOrWhiteSpace(id)) return;

            var latestLog = GetLatestLog(id);
            if (latestLog?.Operation == CorporateCalendar.Logging.Log.OperationType.Edit.ToString())
            {
                _db.Logs.DeleteOnSubmit(latestLog);
                _db.SubmitChanges();
                return;
            }

        }

        private void GetActivityStatus(HttpContext context)
        {
            string response = "";
            context.Response.ContentType = "text/plain";

            string id = context.Request["activityId"];
            if (string.IsNullOrWhiteSpace(id)) return;

            var latestLog = GetLatestLog(id);

            if (latestLog?.Operation == CorporateCalendar.Logging.Log.OperationType.Edit.ToString())
            {
                var userEditingActivity = GetSystemUser(latestLog);
                response = string.Format("{0} is editing this activity. This tab will automatically be closed. Please try re-opening the activity later.", userEditingActivity?.FullName);
                context.Response.Write(response);
                return;
            }

            var newchange = new Log();
            newchange.ActivityId = int.Parse(id);
            newchange.LogType = Convert.ToInt32(CorporateCalendar.Logging.Log.LogType.Message);
            newchange.Operation = CorporateCalendar.Logging.Log.OperationType.Edit.ToString();
            newchange.CreatedBy = CustomPrincipal.Id;
            newchange.CreatedDateTime = DateTime.Now;
            newchange.TableName = "Activity";
            newchange.LastUpdatedBy = newchange.CreatedBy;
            newchange.LastUpdatedDateTime = DateTime.Now;
            newchange.IsActive = true;
            _db.Logs.InsertOnSubmit(newchange);
            _db.SubmitChanges();

            context.Response.Write(response);
        }

        private Log GetLatestLog(string activityId)
            => _db.Logs
                .Where(al => al.ActivityId == int.Parse(activityId))
                .OrderByDescending(l => l.CreatedDateTime).ToList()
                .FirstOrDefault();

        private SystemUser GetSystemUser(Log log) => _db.SystemUsers.FirstOrDefault(u => u.Id == log.CreatedBy);

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}