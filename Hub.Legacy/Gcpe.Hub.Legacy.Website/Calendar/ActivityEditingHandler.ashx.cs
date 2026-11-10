using CorporateCalendar.Data;
using CorporateCalendar.Security;
using System;
using System.Linq;
using System.Transactions;
using System.Web;
using Log = CorporateCalendar.Data.Log;

namespace Gcpe.Hub.Calendar
{
    /// <summary>
    /// Summary description for ActivityEditingHandler
    /// </summary>
    public class ActivityEditingHandler : IHttpHandler
    {

        private static readonly TimeSpan EditingTimeout = TimeSpan.FromMinutes(15);
        private readonly CorporateCalendarDataContext _db;
        private int _editorId;

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
            context.Response.ContentType = "text/plain";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            context.Response.Cache.SetNoStore();

            int activityId;
            Guid editingToken;
            if (!int.TryParse(context.Request["activityId"], out activityId) || activityId <= 0 ||
                !Guid.TryParse(context.Request["editingToken"], out editingToken) || editingToken == Guid.Empty)
            {
                context.Response.StatusCode = 400;
                return;
            }

            _editorId = CustomPrincipal.Id;
            // Serialize lock changes across browsers and web servers without changing the activity.
            using (_db)
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
            {
                if (!_db.ExecuteQuery<int>(
                    "SELECT [Id] FROM [calendar].[Activity] WITH (UPDLOCK, HOLDLOCK) WHERE [Id] = {0}",
                    activityId).Any())
                {
                    context.Response.StatusCode = 404;
                    return;
                }

                switch (context.Request.QueryString["Op"])
                {
                    case "GetActivityStatus":
                        GetActivityStatus(context, activityId, editingToken);
                        break;
                    case "RenewEditingActivity":
                        RenewEditingActivity(context, activityId, editingToken);
                        break;
                    case "CancelEditingActivity":
                        CancelEditingActivity(activityId, editingToken);
                        break;
                    default:
                        context.Response.StatusCode = 400;
                        break;
                }
                transaction.Complete();
            }
        }

        private bool OwnsEditingLog(Log log, Guid editingToken)
            => log != null && log.IsActive && log.RowGuid == editingToken && log.CreatedBy == _editorId;

        private void CancelEditingActivity(int activityId, Guid editingToken)
        {
            var latestLog = GetLatestEditingLog(activityId);
            if (OwnsEditingLog(latestLog, editingToken))
            {
                latestLog.IsActive = false;
                _db.SubmitChanges();
            }
        }

        private void RenewEditingActivity(HttpContext context, int activityId, Guid editingToken)
        {
            var latestLog = GetLatestEditingLog(activityId);
            if (!OwnsEditingLog(latestLog, editingToken))
            {
                context.Response.Write("Your editing session has ended. This tab will automatically be closed. Please try re-opening the activity later.");
                return;
            }

            latestLog.LastUpdatedDateTime = DateTime.Now;
            _db.SubmitChanges();
        }

        private void GetActivityStatus(HttpContext context, int activityId, Guid editingToken)
        {
            var latestLog = GetLatestEditingLog(activityId);

            if (OwnsEditingLog(latestLog, editingToken))
            {
                latestLog.LastUpdatedDateTime = DateTime.Now;
                _db.SubmitChanges();
                return;
            }

            if (latestLog != null && latestLog.IsActive && latestLog.LastUpdatedDateTime > DateTime.Now.Subtract(EditingTimeout))
            {
                var userEditingActivity = GetSystemUser(latestLog);
                context.Response.Write(string.Format("{0} is editing this activity. This tab will automatically be closed. Please try re-opening the activity later.", userEditingActivity?.FullName));
                return;
            }

            var newchange = new Log();
            newchange.ActivityId = activityId;
            newchange.LogType = Convert.ToInt32(CorporateCalendar.Logging.Log.LogType.Message);
            newchange.Operation = CorporateCalendar.Logging.Log.OperationType.Edit.ToString();
            newchange.CreatedBy = _editorId;
            newchange.CreatedDateTime = DateTime.Now;
            newchange.TableName = "Activity";
            newchange.LastUpdatedBy = newchange.CreatedBy;
            newchange.LastUpdatedDateTime = DateTime.Now;
            newchange.IsActive = true;
            newchange.RowGuid = editingToken;
            _db.Logs.InsertOnSubmit(newchange);
            _db.SubmitChanges();
        }

        private Log GetLatestEditingLog(int activityId)
            => _db.Logs
                .Where(al => al.ActivityId == activityId && al.Operation == CorporateCalendar.Logging.Log.OperationType.Edit.ToString())
                .OrderByDescending(l => l.Id)
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
