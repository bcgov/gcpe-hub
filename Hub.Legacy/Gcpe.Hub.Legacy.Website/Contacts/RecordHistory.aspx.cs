using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI.WebControls;
using MediaRelationsDatabase;
using MediaRelationsLibrary;

[ScriptService]
public partial class RecordHistory : System.Web.UI.Page
{
    static string contactAppRoot;
    protected void Page_Load(object sender, EventArgs e)
    {
        contactAppRoot = ResolveUrl("~/Contacts/");
    }

    public static string FormatJsonStringParam(HttpRequest request, string paramName)
    {
        string paramValue = request.QueryString[paramName];
        if (!string.IsNullOrEmpty(paramValue))
        {
            paramValue = "'" + paramValue + "'";
        }
        return "\"" + paramValue + "\"";
    }

    [WebMethod()]
    [ScriptMethod(UseHttpGet = true)]
    public static List<Hashtable> GetHistoryRecords(int skip, int limit, bool sortDescending, string sort, string guid, string type)
    {
        using (MediaRelationsEntities ctx = new MediaRelationsEntities())
        {
            IQueryable<SysLog> logs = ctx.SysLogs.Where(l => l.Action != "71" && l.Action != "72"); // not locked or unlocked
            Guid entityGuid;
            bool filterOnEntityId = Guid.TryParse(guid, out entityGuid);
            if (filterOnEntityId)
            {
                logs = logs.Where(l => l.EntityId == entityGuid);
            }
            else
            {
                if (!string.IsNullOrEmpty(type))
                {
                    if (type.ToLower().Trim() == "contact")
                    {
                        logs = logs.Where(l => l.EntityType == ((int)CommonEventLogging.EntityType.Contact).ToString());
                    }
                    else if (type.ToLower().Trim() == "company")
                    {
                        logs = logs.Where(l => l.EntityType == ((int)CommonEventLogging.EntityType.Company).ToString());
                    }
                }
            }

            string sortField;
            sort = sort != null ? sort.Trim() : "";
            switch (sort.ToLower())
            {
                case "user":
                    sortField = "EventUser";
                    break;
                case "record name":
                    sortField = "EntityData";
                    break;
                case "action":
                    sortField = "Action";
                    break;
                default:
                    sortField = "LogID"; // instead of EventDate for when logging several event at the same time
                    break;
            }

            List<SysLog> sysLogs = LinqDataMethods.OrderBy(logs, sortField, sortDescending, false).Skip(skip).Take(limit).ToList();

            List<Hashtable> results = new List<Hashtable>();
            foreach (SysLog log in sysLogs)
            {
                Hashtable row = new Hashtable();
                results.Add(row);
                int action = int.Parse(log.Action);

                string actionStr = actionStr = Enum.ToObject(typeof(CommonEventLogging.ActivityType), action).ToString();
                actionStr = actionStr.Replace("_", " ");

                int entityType = int.Parse(log.EntityType);
                string entityTypeName = null;
                string recordName = null;
                bool publicReport = entityType == (int)CommonEventLogging.EntityType.PublicReport;
                if (entityType <= (int)CommonEventLogging.EntityType.Contact || publicReport)
                {
                    entityTypeName = Enum.Parse(typeof(CommonEventLogging.EntityType), log.EntityType).ToString();
                    if (action == (int)CommonEventLogging.ActivityType.Record_Edited)
                    {
                        actionStr = entityTypeName + " Edited";
                    }
                    else if (action == (int)CommonEventLogging.ActivityType.Record_Created)
                    {
                        actionStr = entityTypeName + " Created";
                    }
                    if (!filterOnEntityId && !publicReport)
                    {
                        recordName = entityTypeName.Replace("Outlet", "MediaOutlet") + "/View" + entityTypeName + ".aspx?guid=";
                        recordName = "<a href = '" + contactAppRoot + recordName + log.EntityId + "'>" + log.EntityData + "</a>";
                    }
                }

                row["Action"]  = actionStr;
                if (!filterOnEntityId)
                {
                    row["RecordName"] = recordName ?? log.EntityData;
                }
                row["EventUser"] = log.EventUser;
                if (log.EventData.StartsWith("<?xml "))
                {
                    // let's get what we can out of this.
                    var eventData = new List<string>();
                    foreach (string token in new[] { "<delete", "<change", "<add"})
                    {
                        int pos = 0;
                        while (true)
                        {
                            pos = log.EventData.IndexOf(token, pos);
                            if (pos++ == -1) break;
                            int posEnd = log.EventData.IndexOf(">", pos);
                            eventData.Add(log.EventData.Substring(pos, posEnd - pos));
                            if (eventData.Count > 2) break;
                            pos = posEnd;
                        }
                    }
                    log.EventData = string.Join("<br>", eventData);
                }
                row["EventData"] = log.EventData;
                row["EventDate"] = log.EventDate.ToString(CommonMethods.FRIENDLY_DATE_FORMAT_STR);
            }
            return results;
        }
    }
}