using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MediaRelationsDatabase;

namespace MediaRelationsLibrary
{
    public class ReportsLib
    {
        private static object reportLockObj = new object(); // lock so that slot orders don't become messed up

        private string user = null;
        public string User { get { return user; } }

        private bool canCreatePublicReports = false;
        public bool CanCreatePublicReports { get { return canCreatePublicReports; } }

        private bool canDeletePublicReports = false;
        public bool CanDeletePublicReports { get { return canDeletePublicReports; } }

        public ReportsLib()
        {
            user = CommonMethods.GetLoggedInUser();

            Permissions.SiteAction actions = Permissions.GetUserPermissions(Permissions.SiteSection.MediaRelationsCommonReports);
            if ((actions & Permissions.SiteAction.Create) != 0) canCreatePublicReports = true;
            else canCreatePublicReports = false;

            if ((actions & Permissions.SiteAction.Delete) != 0) canDeletePublicReports = true;
            else canDeletePublicReports = false;
        }

        /// <summary>
        /// This is a method that returns if a report name is unique in the system
        /// public - checks against all
        /// private - checks against users
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="isPublic"></param>
        /// <returns></returns>
        public bool GetIsUniqueReportName(string reportName, bool isPublic)
        {
            bool uniqueName = true;

            using (MediaRelationsEntities ctx = new MediaRelationsEntities())
            {
                if (!isPublic)
                {
                    int count = (from r in ctx.Reports where r.ReportName == reportName where r.ReportOwner == User where r.IsPublic == false select r).Count();
                    if (count > 0) uniqueName = false;
                }
                else
                {
                    int count = (from r in ctx.Reports where r.ReportName == reportName where r.IsPublic == true select r).Count();
                    if (count > 0) uniqueName = false;
                }
            }
            return uniqueName;
        }

        /// <summary>
        /// This is the class that creates the report in the database
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="reportQueryString"></param>
        /// <param name="isPublic"></param>
        private void CreateReport(string reportName, string reportQueryString, bool isPublic)
        {
            // get a lock for create report to prevent the slot order from becoming
            // wrong during insertion of reports
            lock (reportLockObj)
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    var obj = (from r in ctx.Reports select r);
                    if (isPublic) obj = obj.Where(x => x.IsPublic == true);
                    else obj = obj.Where(x => x.ReportOwner == User);

                    int highestSlot = 0;
                    if (obj.Count() > 0)
                    {
                        highestSlot = obj.Max(x => x.SortOrder);
                    }

                    int thisSlot = highestSlot + 1;

                    Report report = new Report()
                    {
                        Id = Guid.NewGuid(),
                        ReportName = reportName,
                        ReportOwner = User,
                        ReportQueryString = reportQueryString,
                        IsPublic = isPublic,
                        SortOrder = thisSlot,
                        CreationDate = DateTime.Now
                    };

                    ctx.Reports.Add(report);
                    ctx.SaveChanges();

                    if (isPublic)
                    {
                        CommonEventLogging.WriteActivityLogEntry(CommonEventLogging.ActivityType.Record_Created, CommonEventLogging.EntityType.PublicReport,
                            report.Id, report.ReportQueryString, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                    }
                }
            }
        }

        /// <summary>
        /// This method does the creation of a private report in the system
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="reportQueryString"></param>
        /// <returns></returns>
        public int CreatePrivateReport(string reportName, string reportQueryString)
        {
            int errors = 0;

            if (string.IsNullOrWhiteSpace(reportName)) errors |= 1;

            bool uniqueName = GetIsUniqueReportName(reportName, false);
            if (!uniqueName) errors |= 2;

            if (errors == 0)
            {
                CreateReport(reportName, reportQueryString, false);
            }

            return errors;
        }

        /// <summary>
        /// This method does the creation of a public report in the system
        /// does validation to ensure user can create public reports
        /// </summary>
        /// <param name="reportName"></param>
        /// <param name="reportQueryString"></param>
        /// <returns></returns>
        public int CreatePublicReport(string reportName, string reportQueryString)
        {
            int errors = 0;

            if (!CanCreatePublicReports) errors |= 1024;
            if (string.IsNullOrWhiteSpace(reportName)) errors |= 1;

            bool uniqueName = GetIsUniqueReportName(reportName, true);
            if (!uniqueName) errors |= 2;

            if (!CanCreatePublicReports) errors |= 4;

            if (errors == 0)
            {
                CreateReport(reportName, reportQueryString, true);
            }

            return errors;
        }

        /// <summary>
        /// deletes a report in the system. does validation to ensure the user can delete the report
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public int DeleteReport(Guid reportId)
        {
            int errors = 0;

            lock (reportLockObj)
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    bool wasPublicReport = false;

                    Report report = (from rp in ctx.Reports where rp.Id == reportId select rp).FirstOrDefault();

                    if (report == null) errors |= 1;
                    else
                    {
                        if (report.IsPublic)
                        {
                            wasPublicReport = true;
                            if (!CanDeletePublicReports) errors |= 2;
                        }
                        else
                        {
                            if (!report.ReportOwner.ToLower().Equals(User.ToLower())) errors |= 4;
                        }
                    }

                    if (errors == 0)
                    {
                        ctx.Reports.Remove(report);

                        ctx.SaveChanges();

                        List<Report> reports;
                        if (wasPublicReport)
                        {
                            reports = (from c in ctx.Reports
                                       where c.IsPublic == true
                                       orderby c.SortOrder ascending
                                       select c).ToList(); ;
                        }
                        else
                        {
                            string loggedInUser = CommonMethods.GetLoggedInUser();
                            reports = (from c in ctx.Reports
                                       where c.IsPublic == false
                                       where c.ReportOwner == loggedInUser
                                       orderby c.SortOrder ascending
                                       select c).ToList();
                        }

                        int newSlotOrder = 1;
                        foreach (Report rep in reports)
                        {
                            rep.SortOrder = newSlotOrder;
                            newSlotOrder++;
                        }


                        ctx.SaveChanges();

                        if (wasPublicReport)
                        {
                            CommonEventLogging.WriteActivityLogEntry(CommonEventLogging.ActivityType.Record_Deleted, CommonEventLogging.EntityType.PublicReport,
                                report.Id, report.ReportQueryString, Guid.Empty, "", CommonMethods.GetLoggedInUser());
                        }
                    }

                }

            }

            return errors;
        }

        /// <summary>
        /// This method changes the order of the report in the system. This is used for both 'my reports' and 'reports'.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="moveUp">true moves the item up (decreases the slot number) false moves the item down (increases the slot number)</param>
        private void MoveReport(Guid guid, bool moveUp)
        {
            lock (reportLockObj)
            {
                using (MediaRelationsEntities ctx = new MediaRelationsEntities())
                {
                    Report report = (from rp in ctx.Reports where rp.Id == guid select rp).FirstOrDefault();
                    bool hasPermission = true;

                    if (report.IsPublic)
                    {
                        hasPermission = CanCreatePublicReports;
                    }

                    if (report != null && hasPermission)
                    {

                        // get the lowest slot number available
                        var obj = (from r in ctx.Reports select r);
                        if (report.IsPublic)
                        {
                            obj = obj.Where(r => r.IsPublic == true);
                        }
                        else
                        {
                            obj = obj.Where(r => r.IsPublic == false).Where(r => r.ReportOwner == report.ReportOwner);
                        }

                        int lowestReportSlot = obj.Min(x => x.SortOrder);
                        int highestReportSlot = obj.Max(x => x.SortOrder);

                        if (moveUp)
                        {
                            // decrease the slot number
                            if (report.SortOrder > lowestReportSlot)
                            {
                                Report prevReport = obj.Where(x => x.SortOrder == report.SortOrder - 1).First();
                                prevReport.SortOrder = prevReport.SortOrder + 1;
                                report.SortOrder = report.SortOrder - 1;
                            }
                        }
                        else
                        {
                            // increase the slot number
                            if (report.SortOrder < highestReportSlot)
                            {
                                Report nextReport = obj.Where(x => x.SortOrder == report.SortOrder + 1).First();
                                nextReport.SortOrder = nextReport.SortOrder - 1;
                                report.SortOrder = report.SortOrder + 1;
                            }
                        }

                        ctx.SaveChanges();
                    }
                }
            }
        }

        /// <summary>
        /// This method calls MoveReport to move a report up in the slot order
        /// </summary>
        /// <param name="guid"></param>
        public void MoveReportSlotUp(Guid guid)
        {
            MoveReport(guid, true);
        }

        /// <summary>
        /// this method calls MoveReport to move a report down in slot order
        /// </summary>
        /// <param name="guid"></param>
        public void MoveReportSlotDown(Guid guid)
        {
            MoveReport(guid, false);
        }

        /// <summary>
        /// This is a common method that checks the query string for the operations of 
        /// moving up or down the slot number of reports or deleting reports
        /// </summary>
        public static void PerformPageChecks()
        {
            ReportsLib lib = new ReportsLib();
            HttpRequest Request = HttpContext.Current.Request;
            HttpResponse Response = HttpContext.Current.Response;
            HttpServerUtility Server = HttpContext.Current.Server;

            if (Request.QueryString["delete"] != null)
            {
                string message = "";
                Guid guid;
                if (Guid.TryParse(Request.QueryString["delete"].Trim(), out guid))
                {
                    int errors = lib.DeleteReport(guid);
                    if (errors != 0)
                    {
                        if ((errors & 1) != 0) message += "Error! Report does not exist.";
                        if ((errors & 2) != 0 || (errors & 4) != 0) message += "Error! You do not have permission to delete this report.";
                    }
                    else
                    {
                        message = "Report successfully deleted";
                    }
                }
                else
                {
                    message = "Invalid deletion request";
                }

                Response.Redirect(Request.Url.AbsolutePath + "?message=" + Server.UrlEncode(message));
            }

            if (Request.QueryString["up"] != null)
            {
                Guid guid;
                if (Guid.TryParse(Request.QueryString["up"].Trim(), out guid))
                {
                    lib.MoveReportSlotUp(guid);
                }

                Dictionary<string, string> query = CommonMethods.GetEditableQueryString();
                query.Remove("up");
                Response.Redirect(Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(query));
            }

            if (Request.QueryString["down"] != null)
            {
                Guid guid;
                if (Guid.TryParse(Request.QueryString["down"].Trim(), out guid))
                {
                    lib.MoveReportSlotDown(guid);
                }

                Dictionary<string, string> query = CommonMethods.GetEditableQueryString();
                query.Remove("down");
                Response.Redirect(Request.Url.AbsolutePath + "?" + CommonMethods.GetQueryString(query));
            }
        }
    }
}
