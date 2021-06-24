using System;
using System.Web;
using System.Linq;
using System.Globalization;
using System.Collections.Generic;
using System.Data;
using CorporateCalendar.Data;
using CorporateCalendar.Security;
using Microsoft.Reporting.WinForms;
using Gcpe.Hub.Properties;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace Gcpe.Hub.Calendar
{
    public class ActivityHandler : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        private CustomPrincipal _customPrincipal = null;
        const string IssueColor = "#ccc0d9";
        private static bool? isDetailedLookAheadReport = null;

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

        HttpRequest request = null;
        private IEnumerable<ActiveActivity> activityList = null;
        private IEnumerable<ActiveActivity> ActivityList
        {
            get
            {
                if (activityList == null && request != null)
                {
                    // Create the list so that it does not change on us when the user comes back to the home page and scrolls or reloads
                    int display = int.TryParse(request["display"], out display) ? display : 3;
                    string categoryId = request["category"];
                    string ministryId = request["ministry"];
                    var list = ActivityListProvider.GetFilteredActivities(CustomPrincipal, request, display, categoryId, ministryId);
                    activityList = list.OrderBy(a => a.StartDateTime.Value.Date)
                        .ThenBy(a => a.EndDateTime.Value.Date)
                        .ThenBy(a => a.StartDateTime.Value.ToString("HH:mm"));
                }
                return activityList;
            }
        }
        Dictionary<int, string> initiativesActivityIds = null;
        private Dictionary<int, string> InitiativesActivityIds
        {
            get
            {
                if (initiativesActivityIds == null)
                {
                    initiativesActivityIds = new Dictionary<int, string>();
                    using (var dc = new CorporateCalendarDataContext()) //"param1", "param2", "param3");
                    {
                        var initiatives = dc.Initiatives;
                        foreach (var activityInitiative in dc.ActivityInitiatives)
                        {
                            string shortName = initiatives.FirstOrDefault(i => i.Id == activityInitiative.InitiativeId).ShortName;
                            if (!string.IsNullOrEmpty(shortName))
                            {
                                string previousInitiative;
                                if (initiativesActivityIds.TryGetValue(activityInitiative.ActivityId, out previousInitiative))
                                {
                                    initiativesActivityIds[activityInitiative.ActivityId] = previousInitiative + ", " + shortName;
                                }
                                else
                                {
                                    initiativesActivityIds.Add(activityInitiative.ActivityId, shortName);
                                }
                            }
                        }
                    }
                }
                return initiativesActivityIds;
            }
        }

        bool? isAppOwner = null;
        private bool IsAppOwner
        {
            get
            {
                if (!isAppOwner.HasValue)
                    isAppOwner = CustomPrincipal.IsInApplicationOwnerOrganizations;
                return isAppOwner.Value;
            }
        }

        public void ProcessRequest(HttpContext context)
        {
            // Ensure the user's session is active and get the current context's user information
            if (context.Session.SessionID == null)
            {
                context.Response.Redirect("~/Calendar/CustomErrorPages/SessionTimeout.aspx");
                return;
            }

            request = context.Request;

            string operation = request.QueryString["Op"];
            // TO DO: ReviewSingle - pull this out of the Activity.aspx.cs file

            isDetailedLookAheadReport = request.QueryString["Detailed"] != null && request.QueryString["Detailed"] == "True";

            switch (operation)
            {
                case "ReviewSelected":
                    ProcessReviewSelected(context);
                    break;
                case "DeleteSelected":
                    ProcessDeleteSelected(context);
                    break;
                case "ExportSelected": // Excel Export
                    ExportToExcel("BCGovernmentActivities", context.Response);
                    break;
                case "PlanningReport":
                case "LookAheadReport":
                case "Main30_60_90Report":
                    ProcessReport(context.Response, operation);
                    break;
                case "ClearLAStatus":
                    ClearLAStatus();
                    break;
            }

            // General Request to release a record lock
            switch (context.Request.QueryString["RecordLock"])
            {
                case "Release":
                    throw new NotImplementedException();

                    /* Remove_RecordLocks
                    if (!string.IsNullOrEmpty(context.Request.QueryString["ActivityId"]))
                    {
                        if (!string.IsNullOrEmpty(context.Request.QueryString["PageGuid"]))
                        {
                            var test = context.Request.QueryString["ActivityId"];
                            int id = Convert.ToInt32(context.Request.QueryString["ActivityId"]);
                            Guid pageId = Guid.Parse(context.Request.QueryString["PageGuid"]);
                            CorporateCalendar.Utility.Program.ReleaseRecordLockByPageGUID(id, pageId);
                        }
                    }
                    break;
                    */
            }
        }

        private void ProcessReviewSelected(HttpContext context)
        {
            string[] ids = context.Request.QueryString["ids"].TrimEnd(',').Split(',');
            string lastLoadTime = context.Request.QueryString["lastLoadedDatetime"];
            List<string> skipIds = new List<string>();

            using (var dc = new CorporateCalendarDataContext()) //"param1", "param2", "param3");
            {
                foreach (string activityId in ids)
                {
                    string[] splitActivityId = activityId.Split('-');

                    /* Remove_RecordLocks
                    isActivityLocked = CorporateCalendar.Utility.Program.CheckForRecordLock(typeof(CorporateCalendar.Data.Activity), splitActivityId[1]);

                    if (!isActivityLocked) {
                    */
                    var activity = dc.Activities.Single(p => p.Id == Convert.ToInt32(splitActivityId[1]));
                    var ministry = dc.Ministries.Single(p => activity.ContactMinistryId != null && p.Id == activity.ContactMinistryId && p.IsActive);

                    if (!string.IsNullOrWhiteSpace(lastLoadTime))
                    {
                        CultureInfo provider = CultureInfo.InvariantCulture;
                        DateTime loadtime;
                        if (!DateTime.TryParseExact(lastLoadTime, "yyyy-MM-ddTHH:mm:ss.fffffff", provider, DateTimeStyles.None, out loadtime))
                        {
                            skipIds.Add(activityId);
                            continue;
                        }
                        DateTime comparedTime = activity.LastUpdatedDateTime ?? activity.CreatedDateTime ?? DateTime.MinValue;

                        if (DateTime.Compare(loadtime, comparedTime) < 0)
                        {
                            skipIds.Add(activityId);
                            continue;
                        }
                    }
                    //activity.IsReviewed = true;
                    if (!activity.IsActive && activity.IsActiveNeedsReview)
                    {
                        activity.IsActiveNeedsReview = false;
                    }
                    else
                    {
                        activity.StatusId = 2; // Reviewed status

                        //Need to also mark the columns that flag columns
                        activity.IsTitleNeedsReview = false;
                        activity.IsDetailsNeedsReview = false;
                        activity.IsCityNeedsReview = false;
                        activity.IsRepresentativeNeedsReview = false;
                        activity.IsStartDateNeedsReview = false;
                        activity.IsEndDateNeedsReview = false;
                        activity.IsCategoriesNeedsReview = false;
                        activity.IsCommMaterialsNeedsReview = false;
                        activity.IsStrategyNeedsReview = false;
                        activity.IsSchedulingConsiderationsNeedsReview = false;
                        activity.IsSignificanceNeedsReview = false;
                        activity.IsInternalNotesNeedsReview = false;
                        activity.IsPremierRequestedNeedsReview = false;
                        activity.IsVenueNeedsReview = false;
                        activity.IsDigitalNeedsReview = false;
                        activity.IsEventPlannerNeedsReview = false;
                        activity.IsTranslationsRequiredNeedsReview = false;
                        activity.IsInitiativesNeedsReview = false;
                        activity.IsTagsNeedsReview = false;
                        activity.IsLeadOrganizationNeedsReview = false;
                        activity.IsDistributionNeedsReview = false;
                        activity.IsOriginNeedsReview = false;

                    }
                    string dateTimeIconHtml = string.Format("<img src=\"images/calendar-edit-icon.png\" title=\"{0}\" align=\"absmiddle\" />&nbsp;", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

                    // Update News Feed table
                    var newsFeed = new CorporateCalendar.Data.NewsFeed
                    {
                        ActivityId = activity.Id,
                        MinistryId =
                            ministry.Id,
                        Text =
                            string.Format(
                                "{7}<a href=\"mailto:{2}\" style=\"color: Blue\">{0}</a> reviewed activity <a href=\"Activity.aspx?ActivityId={1}\" style=\"color: Blue\" title=\"{6}\">{3}-{1}</a> at {4} on {5}.",
                                string.Format("{0} {1}",
                                                CustomPrincipal.FirstName,
                                                CustomPrincipal.LastName),
                                activity.Id.ToString(), CustomPrincipal.Email,
                                ministry.Abbreviation,
                                DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString(), activity.Title, dateTimeIconHtml),
                        LastUpdatedBy = CustomPrincipal.Id,
                        CreatedBy = CustomPrincipal.Id,
                        Description = "review",
                        CreatedDateTime = DateTime.Now,
                        IsActive = true
                    };


                    dc.NewsFeeds.InsertOnSubmit(newsFeed);
                    dc.SubmitChanges();

                }
                string returnMsg = "";

                if (skipIds.Count > 0)
                {
                    returnMsg = string.Join(", ", skipIds);

                    if (skipIds.Count > 1)
                        returnMsg += " have not been reviewed.  The activities may have been changed by another user.  Check the details before reviewing these activities again.";
                    else
                        returnMsg += " has not been reviewed.  The activity may have been changed by another user.  Check the details before reviewing the activity again.";
                }

                context.Response.Write(returnMsg);
            }
        }

        private void ProcessDeleteSelected(HttpContext context)
        {
            string[] ids = context.Request.QueryString["ids"].TrimEnd(',').Split(',');
            using (var dc = new CorporateCalendarDataContext()) //"param1", "param2", "param3");
            {
                foreach (string activityId in ids)
                {
                    string[] splitActivityId = activityId.Split('-');

                    /* Remove_RecordLocks
                    isActivityLocked = CorporateCalendar.Utility.Program.CheckForRecordLock(typeof(CorporateCalendar.Data.Activity), splitActivityId[1]);

                    if (!isActivityLocked) {
                    */
                    var activity = dc.Activities.Single(p => p.Id == Convert.ToInt32(splitActivityId[1]));
                    var ministry = dc.Ministries.Single(p => activity.ContactMinistryId != null && p.Id == activity.ContactMinistryId && p.IsActive);


                    activity.IsActive = false;
                    activity.IsActiveNeedsReview = true;
                    //activity.IsReviewed = false;

                    string dateTimeIconHtml = string.Format("<img src=\"images/calendar-edit-icon.png\" title=\"{0}\" align=\"absmiddle\" />&nbsp;", DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString());

                    // Update News Feed table

                    var newsFeed = new NewsFeed
                    {
                        ActivityId = activity.Id,
                        MinistryId =
                            ministry.Id,
                        Text =
                            string.Format(
                                "{7}<a href=\"mailto:{2}\" style=\"color: Blue\">{0}</a> deleted activity <a href=\"Activity.aspx?ActivityId={1}\" style=\"color: Blue\" title=\"{6}\">{3}-{1}</a> at {4} on {5}.",
                                string.Format("{0} {1}",
                                              CustomPrincipal.FirstName,
                                              CustomPrincipal.LastName),
                                activity.Id.ToString(), CustomPrincipal.Email,
                                ministry.Abbreviation,
                                DateTime.Now.ToShortTimeString(), DateTime.Now.ToShortDateString(), activity.Title, dateTimeIconHtml),
                        LastUpdatedBy = CustomPrincipal.Id,
                        CreatedBy = CustomPrincipal.Id,
                        Description = "delete",
                        CreatedDateTime = DateTime.Now,
                        IsActive = true
                    };


                    dc.NewsFeeds.InsertOnSubmit(newsFeed);
                    dc.SubmitChanges();
                }
            }
        }


        private System.Web.UI.WebControls.TableCell NewCellWithBorder(string text)
        {
            return new System.Web.UI.WebControls.TableCell
            {
                Text = text,
                BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid,
                BorderColor = System.Drawing.Color.LightGray,
                VerticalAlign = System.Web.UI.WebControls.VerticalAlign.Top
            };
        }


        private void AddHeaderCell(System.Web.UI.WebControls.TableRow row, string header, int width = 50)
        {
            /*
             * 
             * Excel Spreadsheet style cut-and-paste for header row
             * .xl66
                {mso-style-parent:style0;
                color:white;
                font-weight:700;
                text-align:center;
                border:.5pt solid windowtext;
                background:#333399; // similar too dark slate grey which is #483D8B
                mso-pattern:black none;
                mso-rotate:45;}

             * */

            System.Web.UI.WebControls.TableCell hcell = new System.Web.UI.WebControls.TableCell();
            hcell.Text = header;
            hcell.BackColor = System.Drawing.ColorTranslator.FromHtml("#31869b");  // turquoise
            hcell.Font.Bold = true;
            hcell.ForeColor = System.Drawing.Color.White;
            //hcell.BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid;
            //hcell.BorderWidth = 1;
            //hcell.VerticalAlign =
            hcell.Style.Add("mso-rotate", "45");
            hcell.Style.Add("text-align", "left");
            hcell.Style.Add("height", "85");
            hcell.Style.Add("width", width.ToString());
            hcell.Style.Add("font-size", "11pt");

            row.Cells.Add(hcell);

            //var hcell = new System.Web.UI.WebControls.TableHeaderCell { Text = header, BackColor = System.Drawing.Color.LightGray };
            //hcell.Font.Bold = true;
            //hcell.BorderStyle = System.Web.UI.WebControls.BorderStyle.Solid;
            //hcell.BorderWidth = 1;
            //row.Cells.Add(hcell);
        }

        private void AddCell(System.Web.UI.WebControls.TableRow head, string text, string color, int columnSpan)
        {
            var cell = new System.Web.UI.WebControls.TableCell();
            cell.Text = text;
            cell.ColumnSpan = columnSpan;
            cell.Style.Add("color", color);
            cell.Style.Add("font-weight", "bold");
            //cell.Style.Add("white-space", "nowrap");
            head.Cells.Add(cell);
        }

        const string EXCELBRK = "<br style='mso-data-placement:same-cell'>";
        private void ExportToExcel(string fileName, HttpResponse response)
        {
            //The Clear method erases any buffered HTML output.
            response.Clear();
            response.Buffer = true;
            response.ContentEncoding = System.Text.Encoding.Unicode;
            response.BinaryWrite(System.Text.Encoding.Unicode.GetPreamble());
            //The AddHeader method adds a new HTML header and value to the response sent to the client.
            response.AddHeader("content-disposition", string.Format("attachment; filename={0}", fileName + ".xls"));
            //The ContentType property specifies the HTTP content type for the response.
            response.ContentType = "application/vnd.ms-excel";

            var table = new System.Web.UI.WebControls.Table();
            table.Style.Add("font-size", "85%");
            var head = new System.Web.UI.WebControls.TableRow();
            var foot = new System.Web.UI.WebControls.TableRow();

            AddCell(head, "Province of BC. Corporate Calendar DRAFT & CONFIDENTIAL", "red", 5);
            AddCell(head, GetDateRangeForExportHeading(), "black", 5);
            AddCell(head, "Printed: " + DateTime.Now.ToString("ddd, MMM d h:mm tt", CultureInfo.CreateSpecificCulture("en-US")), "black", 4);

            table.Rows.Add(head);

            //Implements a TextWriter for writing information to a string. The information is stored in an underlying StringBuilder.
            using (var sw = new System.IO.StringWriter())
            {
                //Writes mark-up characters and text to an ASP.NET server control output stream. This class provides formatting capabilities that ASP.NET server controls use when rendering markup to clients.
                using (var htw = new System.Web.UI.HtmlTextWriter(sw))
                {
                    //  Create a form to contain the List
                    var row = new System.Web.UI.WebControls.TableRow();
                    table.Rows.Add(row);

                    //  add each of the data item to the table
                    foreach (var activity in ActivityList)
                    {
                        var tableRow = new System.Web.UI.WebControls.TableRow();

                        var idTableCell = NewCellWithBorder(activity.Id.ToString());
                        var ministryTableCell = NewCellWithBorder("<strong>" + activity.Ministry + "<strong>");

                        var categories = (activity.Categories ?? "").Trim();
                        if (activity.IsIssue)
                        {
                            categories = "Issue, " + categories;
                            categories = categories.Replace(", FYI Only", "");
                            categories = categories.TrimEnd(',', ' ');
                        }

                        var title = activity.Title.Replace("<", "&lt;").Replace(">", "&gt;");
                        var summaryTableCell = NewCellWithBorder(activity.Details.Replace("<", "&lt;").Replace(">", "&gt;"));

                        if (activity.IsConfidential)
                        {
                            summaryTableCell.Text = "<span style='color:darkred'>Not for Look Ahead</span> " + summaryTableCell.Text;
                        }

                        var significance = activity.Significance;
                        if (!string.IsNullOrEmpty(activity.Strategy))
                        {
                            significance += EXCELBRK + EXCELBRK + "<i>Strategy:</i> " + activity.Strategy;
                        }

                        if (activity.GovernmentRepresentative == null)
                            activity.GovernmentRepresentative = "";

                        if (activity.IsPremierRequestedOrConfirmed)
                            activity.GovernmentRepresentative = "Premier" + (activity.GovernmentRepresentative == "" ? "" : ", ") + activity.GovernmentRepresentative;

                        var dt = ActivityListProvider.FriendlyDateTimeRange(activity, true).Replace("-", " -"); // Excel does not break on dashes

                        // City
                        string cityAndVenue = activity.CityOrOther;
                        if (!string.IsNullOrEmpty(activity.Venue))
                            cityAndVenue += EXCELBRK + activity.Venue;
                        var cityTableCell = NewCellWithBorder(cityAndVenue);

                        var leadOrganizationTableCell = NewCellWithBorder(activity.LeadOrganization);
                        var commContactTableCell = NewCellWithBorder(activity.ContactName + EXCELBRK + activity.ContactNumber);

                        TableCell eventPlannerTableCell = null;
                        using (var dc = new CorporateCalendarDataContext())
                        {
                            var dbEventPlanner = dc.EventPlanners.FirstOrDefault(e => e.Id == activity.EventPlannerId);
                            eventPlannerTableCell = dbEventPlanner == null ? NewCellWithBorder("") : NewCellWithBorder(dbEventPlanner.Name);
                        }


                        tableRow.Cells.Add(idTableCell);
                        tableRow.Cells.Add(ministryTableCell);
                        tableRow.Cells.Add(NewCellWithBorder("<strong>" + categories + "</strong>"));
                        tableRow.Cells.Add(NewCellWithBorder(dt));
                        tableRow.Cells.Add(NewCellWithBorder("<strong>" + title + "</strong>"));
                        tableRow.Cells.Add(summaryTableCell);
                        tableRow.Cells.Add(NewCellWithBorder(significance));
                        tableRow.Cells.Add(eventPlannerTableCell);
                        tableRow.Cells.Add(NewCellWithBorder(activity.Schedule));
                        tableRow.Cells.Add(NewCellWithBorder(activity.CommunicationsMaterials));
                        tableRow.Cells.Add(leadOrganizationTableCell);
                        tableRow.Cells.Add(commContactTableCell);
                        tableRow.Cells.Add(NewCellWithBorder(activity.GovernmentRepresentative));
                        tableRow.Cells.Add(cityTableCell);
                        tableRow.Cells.Add(NewCellWithBorder(activity.Keywords));
                        tableRow.Cells.Add(NewCellWithBorder(activity.PremierRequested));

                        table.Rows.Add(tableRow);
                    }
                    AddHeaderCell(row, "ID", 45);
                    AddHeaderCell(row, "Ministry");
                    AddHeaderCell(row, "Categories", 70);
                    AddHeaderCell(row, "Date & Time", 85);
                    AddHeaderCell(row, "Title", 90);
                    AddHeaderCell(row, "Summary", 250);
                    AddHeaderCell(row, "Significance and Strategy", 180);
                    AddHeaderCell(row, "Event Planner", 70);
                    AddHeaderCell(row, "Scheduling Notes", 90);
                    AddHeaderCell(row, "Comm. Materials", 70);
                    AddHeaderCell(row, "Lead Org");
                    AddHeaderCell(row, "Comm. Contact");
                    AddHeaderCell(row, "Govt Rep.", 55);
                    AddHeaderCell(row, "City", 65);
                    AddHeaderCell(row, "Tags", 65);
                    AddHeaderCell(row, "Premier Requested", 65);

                    string footDisclaimer = "CONFIDENTIALITY NOTICE:  This information, including any attachments, is confidential.  It is intended only for the use of the person or persons to whom it is addressed or shared with, unless I have expressly authorized otherwise.  If you have received this data extract in error, please discard the document, including any related information or attachments, and notify the Corporate Calendar Administrator immediately by email or telephone.";
                    AddCell(foot, footDisclaimer, "red", 10);
                    table.Rows.Add(foot);

                    //  render the table into the htmlwriter
                    table.RenderControl(htw);
                    //  render the htmlwriter into the response
                    response.Write(sw);
                    response.Flush();
                }
            }
        }

        private string GetDateRangeForExportHeading()
        {
            string dateRangeHeading = "Date Range Selected: ";
            string fromDate = request.QueryString["datefrom"]; // MM/dd/YYYY eg. 08/20/2012
            string toDate = request.QueryString["dateto"];     // MM/dd/YYYY eg. 08/20/2012

            // convert dates to easy readable format and handle unspecified dates
            bool hasFromDate = !string.IsNullOrEmpty(fromDate);
            fromDate = (hasFromDate ? Convert.ToDateTime(fromDate) : DateTime.Today).ToString("MMM dd, yyyy"); // eg. Aug 20, 2012

            if (string.IsNullOrEmpty(toDate)) toDate = " date-forward";
            else toDate = " to " + Convert.ToDateTime(toDate).ToString("MMM dd, yyyy"); // eg. Aug 20, 2012

            return dateRangeHeading + fromDate + toDate;
        }

        private static void GeneratePlannedActivities(DataTable activitiesTable, IEnumerable<ActiveActivity> activityList)
        {
            var scheduleColumn = activitiesTable.Columns.Add("Schedule", typeof(string));
            var tagsColumn = activitiesTable.Columns.Add("Tags", typeof(string));
            var summaryColumn = activitiesTable.Columns.Add("Summary", typeof(string));
            var significanceColumn = activitiesTable.Columns.Add("Significance", typeof(string));
            var ministryColumn = activitiesTable.Columns.Add("Ministry", typeof(string));
            var activityIdColumn = activitiesTable.Columns.Add("ActivityId", typeof(string));
            var detailsColumn = activitiesTable.Columns.Add("Details", typeof(string));

            //List<CorporateCalendar.PlanningReportDataSet.ActivitiesRow> rows = new List<CorporateCalendar.PlanningReportDataSet.ActivitiesRow>();

            foreach (ActiveActivity activity in activityList)
            {
                string date = ActivityListProvider.FriendlyDateTimeRange(activity, true);
                DataRow row = AddNewRow(scheduleColumn, date + (string.IsNullOrEmpty(activity.Schedule) ? "" : "<br/>" + activity.Schedule));

                bool HasPremierInfo = !string.IsNullOrEmpty(activity.PremierRequested);
                string tags = HasPremierInfo ? "<br><b>Premier Requested: </b> " + activity.PremierRequested.Replace("Premier ", "") : "";
                if (!string.IsNullOrEmpty(activity.Keywords))
                {
                    var keywords = activity.Keywords.Split(',').OrderBy(k => k);
                    var sortedTag = keywords.Where(k => k.StartsWith(" HQ")).ToList();
                    sortedTag.AddRange(keywords.Where(k => !k.StartsWith(" HQ")));
                    tags += "<br><b>Tags:</b>" + string.Join(",", sortedTag);
                }

                row[tagsColumn] = string.IsNullOrEmpty(tags) ? "" : "<br>" + tags;

                string headline = activity.City != null && activity.City != "TBD" ? activity.CityOrOther : "";
                headline = headline.Replace(", BC", "").Trim();
                headline += (headline == "" ? "" : " - ") + activity.Title;

                string summary = "<b>" + headline + "</b>";
                summary += "<br />";
                if (activity.NotForLookAhead)
                {
                    summary += "<b><span style='color:darkred'>Not for Look Ahead</span>&nbsp;</b>";
                }
                row[summaryColumn] = summary + activity.Details;

                string significance = string.Empty;
                if ((activity.Categories != null && activity.Categories.Contains("Issue") || activity.IsIssue))
                {
                    significance = "<b>Issue</b>";
                }
                else if (activity.Categories != null && activity.Categories.Contains("FYI Only"))
                {
                    significance = "FYI Only";
                }

                row[significanceColumn] = significance == "" ? "" : significance + "<br />";
                row[significanceColumn] += activity.Significance;

                row[ministryColumn] = activity.Ministry;
                row[activityIdColumn] = activity.Id;
                //row["Details"] += "<br />";
                row[detailsColumn] = " <br /><span style='font-size:9pt'>";
                row[detailsColumn] += ActivityListProvider.GetCreatedOrUpdatedMessage(activity.Status == "New", activity.CreatedDateTime, "", activity.LastUpdatedDateTime);

                row[detailsColumn] += " </span>";
            }

            //foreach (var row in rows)//.OrderBy(e => e.Date).ThenByDescending(e => e.Internal).ThenBy(e => e.StartTime))
            //    activitiesTable.Rows.Add(row);
        }

        Dictionary<DateTime, DataTable> dailyEventsAndSpeeches = null;

        private void ProcessReport(HttpResponse response, string operation)
        {
            response.CacheControl = "no-cache";

            //response.AddHeader("content-disposition", "attachment; filename=" + operation + ".doc");
            var reportViewer = new ReportViewer();
            LocalReport localReport = reportViewer.LocalReport;

            reportViewer.ProcessingMode = ProcessingMode.Local;
            localReport.EnableHyperlinks = true;
            var reportData = new DataTable();
            localReport.ReportEmbeddedResource = "Gcpe.Hub.Calendar.Reports." + operation + ".rdlc";

            string siteUrl = HttpContext.Current.Request.Url.AbsoluteUri;
            siteUrl = siteUrl.Substring(0, siteUrl.IndexOf("ActivityHandler", StringComparison.Ordinal));
            var parameters = new List<ReportParameter>();
            parameters.Add(new ReportParameter("SiteURL", siteUrl + "Activity.aspx?ActivityId="));

            if (operation == "PlanningReport")
            {
                GeneratePlannedActivities(reportData, ActivityList);
            }
            else
            {
                bool is30_60_90 = (operation == "Main30_60_90Report");

                DateTime reportDate = DateTime.Today;
                if (!DateTime.TryParse(request.QueryString["datefrom"], out reportDate))
                {
                    reportDate = DateTime.Today;
                }
                if (is30_60_90)
                {
                    reportDate = reportDate.AddDays(1 - reportDate.Day); // make sure it's beginning of the month
                }
                DateTime toDate;
                bool includeLTOutlook = !DateTime.TryParse(request.QueryString["dateto"], out toDate);
                bool thisdayonly = request.QueryString["thisdayonly"] == "true";
                if (includeLTOutlook || thisdayonly)
                {
                    includeLTOutlook = !thisdayonly;
                    toDate = thisdayonly ? reportDate : reportDate.AddDays(is30_60_90 ? 90 : 59);

                    if (isDetailedLookAheadReport == true)
                    { // reassigning toDate so as not to break existing code
                        toDate = thisdayonly ? reportDate : reportDate.AddMonths(1);
                    }
                }
                DataColumn dailyPageBreakColumn = null, hasNoActivitiesThatDayColumn = null;
                if (!is30_60_90)
                {
                    const string dateFormat = "dddd, MMM. d";
                    string title = reportDate.ToString(reportDate.Year != toDate.Year ? dateFormat : dateFormat + ", yyyy");
                    if (!thisdayonly)
                    {
                        title += " to " + toDate.ToString(dateFormat + ", yyyy");
                    }
                    parameters.Add(new ReportParameter("ReportTitle", title));
                    parameters.Add(new ReportParameter("IsAppOwner", IsAppOwner.ToString()));
                    parameters.Add(new ReportParameter("IncludeLTOutlook", includeLTOutlook.ToString()));
                    parameters.Add(new ReportParameter("CoverImage", "file:" + request.PhysicalApplicationPath + Settings.Default.CalendarLookAheadCoverImg));
                    localReport.EnableExternalImages = true;
                    dailyPageBreakColumn = reportData.Columns.Add("DailyPageBreak", typeof(bool));
                    hasNoActivitiesThatDayColumn = reportData.Columns.Add("NoActivitiesThatDay", typeof(string));
                    dailyEventsAndSpeeches = new Dictionary<DateTime, DataTable>();
                }
                /*else
                {
                    using (var dc = new CorporateCalendarDataContext())
                    {
                        string ministry = dc.Ministries.First(p => p.Id == systemUserMinistries.First()).LongName;
                        if (!ministry.Contains("GCPE ") && !ministry.Contains("Office") && !ministry.Contains("Secretariat") && ministry != "Citizen Engagement")
                        {
                            ministry = "Ministry of " + ministry;
                        }
                        parameters.Add(new ReportParameter("Ministry", ministry + " - " ));
                    }
                }*/

                var reportDateColumn = reportData.Columns.Add("ReportDate", typeof(DateTime));

                int numActivitiesSinceBreak = 0;
                while (reportDate <= toDate)
                {
                    DataRow row = AddNewRow(reportDateColumn, reportDate);
                    if (is30_60_90)
                    {
                        reportDate = reportDate.AddMonths(1);
                    }
                    else
                    {
                        DataTable activities = new DataTable();
                        var dateColumn = activities.Columns.Add("Date", typeof(string));
                        var titleDetailsColumn = activities.Columns.Add("TitleDetails", typeof(string));
                        var activityIdColumn = activities.Columns.Add("ActivityId", typeof(string));
                        var ministryColumn = activities.Columns.Add("Ministry", typeof(string));
                        GenerateLookAheadActivities(ActivityList, IsAppOwner, reportDate, null,
                            InitiativesActivityIds, dateColumn, titleDetailsColumn, activityIdColumn, ministryColumn, null);
                        int numActivitiesInPage = activities.Rows.Count;
                        if (activities.Rows.Count != 0)
                        {
                            numActivitiesInPage += 2; // 1 for the date header and 1 for the section header
                        }
                        else
                        {
                            row[hasNoActivitiesThatDayColumn] = "No Activities for " + reportDate.ToString("dddd, MMMM d, yyyy");
                        }
                        numActivitiesSinceBreak += numActivitiesInPage;
                        if (IsAppOwner && (reportDate.DayOfWeek != DayOfWeek.Saturday || (numActivitiesSinceBreak + numActivitiesInPage) > 16) || reportDate == toDate)
                        {
                            numActivitiesSinceBreak = 0;
                            row[dailyPageBreakColumn] = true;
                        }
                        dailyEventsAndSpeeches[reportDate] = activities;

                        reportDate = reportDate.AddDays(1);
                    }
                }
                localReport.SubreportProcessing += new SubreportProcessingEventHandler(SubreportEventHandler);
            }
            localReport.SetParameters(parameters);
            localReport.DataSources.Add(new ReportDataSource(operation.Replace("Report", "Data"), reportData));

            string mimeType, encoding, extension;
            string[] streamids;
            Warning[] warnings;

            //Format can be "Excel", "Word", "PDF", or "Image"
            string format = request.QueryString["format"] ?? "PDF";

            byte[] bytes = localReport.Render(format, null, out mimeType, out encoding, out extension, out streamids, out warnings);

            response.ContentType = mimeType;

            response.BinaryWrite(bytes);
        }

        private void SubreportEventHandler(object sender, SubreportProcessingEventArgs e)
        {
            string dataSourceName = e.DataSourceNames.First();
            var firstParameter = e.Parameters.First().Values[0];
            if (dataSourceName == "LookAheadLegendData")
            {
                DataTable sections = new DataTable();
                var colorColumn = sections.Columns.Add("Color", typeof(string));
                var sectionColumn = sections.Columns.Add("Section", typeof(string));

                DataRow row = AddNewRow(sectionColumn, "<b>Events, Speeches and Releases</b> (Inside Government)");
                row[colorColumn] = "#558abd";
                row = AddNewRow(sectionColumn, "<b>Issues and Reports</b>");
                row[colorColumn] = IssueColor;
                row = AddNewRow(sectionColumn, "<b>Consultations and Dialogues</b>");
                row[colorColumn] = "#daeef3";
                row = AddNewRow(sectionColumn, "<b>In the News</b> (Outside Government)");
                row[colorColumn] = "#e8f3a9";
                row = AddNewRow(sectionColumn, "<b>Awareness Dates</b>");
                row[colorColumn] = "#eaf1dd";
                if (firstParameter == "True")
                {
                    row = AddNewRow(sectionColumn, "<b>Long Term Outlook</b>");
                    row[colorColumn] = "#edf2f8";
                }
                e.DataSources.Add(new ReportDataSource(dataSourceName, sections));
                return;
            }

            DateTime reportDate;
            bool hasReportDate = DateTime.TryParse(firstParameter, out reportDate);

            if (dataSourceName == "LookAheadEventSpeechRlsData")
            {
                // already been calculated to see where PageBreaks are needed
                e.DataSources.Add(new ReportDataSource(dataSourceName, dailyEventsAndSpeeches[reportDate]));
                return;
            }

            DataTable activities = new DataTable();
            var dateColumn = activities.Columns.Add("Date", typeof(string));

            var titleDetailsColumn = activities.Columns.Add("TitleDetails", typeof(string));
            var activityIdColumn = activities.Columns.Add("ActivityId", typeof(string));
            var ministryColumn = activities.Columns.Add("Ministry", typeof(string));
            if (dataSourceName == "Monthly30_60_90Data")
            {
                GenerateMonthly30_60_90(ActivityList, reportDate, dateColumn, titleDetailsColumn, activityIdColumn, ministryColumn);
                activityList = activityList.Skip(activities.Rows.Count); // don't duplicate activities spanning over the next month
            }
            else
            {
                DateTime? outlookDate = null;
                if (string.IsNullOrEmpty(request.QueryString["dateto"]))
                {
                    DateTime fromDate;
                    outlookDate = (DateTime.TryParse(request.QueryString["datefrom"], out fromDate) ? fromDate : DateTime.Today).AddDays(60);
                }

                if (dataSourceName == "LookAheadAwarenessConsultationOutlookData")
                {
                    var flagColumn = activities.Columns.Add("Flag", typeof(string));

                    foreach (ActiveActivity activity in ActivityList)
                    {
                        if (BelongsToAwarenessConsultationReport(activity, outlookDate) != firstParameter) continue;
                        string date;
                        if (firstParameter == "Consultations and Dialogues")
                        {
                            if (!activity.IsConfirmed) continue;
                            DateTime end = activity.EndDateTime.Value.Date;
                            bool ongoing = end >= DateTime.Now.AddYears(1);
                            date = ongoing ? "Ongoing" : "Closes " + ActivityListProvider.FriendlyDate(end, false);
                        }
                        else
                        {
                            if (firstParameter == "Long Term Outlook" && IsAppOwner && !activity.IsForLongTermOutlook) continue;
                            date = ActivityListProvider.FriendlyDateTimeRange(activity, false);
                        }
                        string titleDetails = FormatHqComments(activity, IsAppOwner);
                        if (titleDetails == null)
                        {
                            titleDetails = firstParameter == "Awareness Dates" ? FormatTitle(activity) : FormatTitleDetails(activity, false);
                        }
                        titleDetails += FormatInitiative(activity, initiativesActivityIds);
                        DataRow row = AddNewDetailsRow(titleDetailsColumn, titleDetails, date.StartsWith("Closes", StringComparison.Ordinal) ? (int?)0 : null);
                        row[dateColumn] = date;
                        row[activityIdColumn] = activity.Id;
                        row[ministryColumn] = activity.Ministry.Replace("GCPEHQ", "HQ");
                        row[flagColumn] = FormatFlag(activity);
                    }
                }
                else
                {
                    var categoryColumn = activities.Columns.Add("Category", typeof(string));
                    GenerateLookAheadActivities(ActivityList, IsAppOwner, hasReportDate ? (DateTime?)reportDate : null, outlookDate, InitiativesActivityIds,
                        dateColumn, titleDetailsColumn, activityIdColumn, ministryColumn, categoryColumn);
                }
            }
            e.DataSources.Add(new ReportDataSource(dataSourceName, activities));
        }

        private static string BelongsToAwarenessConsultationReport(ActiveActivity activity, DateTime? outlookDate)
        {
            if (activity.Ministry == "CITENG") return "Consultations and Dialogues";
            //if (activity.Title.Contains("SLIDER:")) return "Sliders";
            if (activity.Categories != null && activity.Categories.Contains("Awareness Day / Week / Month")) return "Awareness Dates";
            if (outlookDate.HasValue && activity.StartDateTime > outlookDate) return "Long Term Outlook";
            return null;
        }

        public static void GenerateMonthly30_60_90(IEnumerable<ActiveActivity> activityList, DateTime startDate,
            DataColumn dateColumn, DataColumn titleDetailsColumn, DataColumn activityIdColumn, DataColumn ministryColumn)
        {
            var activitiesColumns = dateColumn.Table.Columns;
            var releaseColumn = activitiesColumns.Add("Release", typeof(string));
            var colorColumn = activitiesColumns.Add("Color", typeof(string));
            var contactColumn = activitiesColumns.Add("Contact", typeof(string));
            var updatedColumn = activitiesColumns.Add("Updated", typeof(string));

            DateTime endDate = startDate.AddMonths(1);

            foreach (ActiveActivity activity in activityList)
            {
                if (activity.StartDateTime >= endDate) break;

                DataRow row = AddNewDetailsRow(titleDetailsColumn, FormatTitleDetails(activity, true), null);
                row[dateColumn] = ActivityListProvider.FriendlyDateTimeRange(activity, true);
                row[activityIdColumn] = activity.Id;
                row[ministryColumn] = activity.Ministry;
                if (activity.IsIssue)
                    row[colorColumn] = IssueColor;

                if (!string.IsNullOrEmpty(activity.ContactName))
                {
                    row[contactColumn] += activity.ContactName;
                    row[updatedColumn] += $"<br ><br > <span style='font-size:8pt'>{ActivityListProvider.GetCreatedOrUpdatedMessage(activity.Status == "New", activity.CreatedDateTime, "", activity.LastUpdatedDateTime)}</span>";
                }
                string materials = activity.CommunicationsMaterials;
                if (!string.IsNullOrEmpty(activity.Strategy))
                {
                    materials = activity.Strategy + "<br ><br >" + materials;
                }

                if (!string.IsNullOrEmpty(materials)) row[releaseColumn] = "<span style='font-size:9pt'>" + materials + "</span>";
            }
        }

        public static void GenerateLookAheadActivities(IEnumerable<ActiveActivity> activityList, bool isAppOwner,
            DateTime? reportDate, DateTime? outlookDate, IDictionary<int, string> initiativesActivityIds,
            DataColumn dateColumn, DataColumn titleDetailsColumn, DataColumn activityIdColumn, DataColumn ministryColumn, DataColumn categoryColumn)
        {
            var activitiesColumns = dateColumn.Table.Columns;
            var releaseColumn = activitiesColumns.Add("Release", typeof(string));
            var flagColumn = activitiesColumns.Add("Flag", typeof(string));
            var colorColumn = activitiesColumns.Add("Color", typeof(string));
            //int numInTheNews = 0;

            DateTime? reportEndDate = reportDate?.AddDays(1);

            bool inTheNews = categoryColumn != null;
            int posOfFirstBlock = 0; // place multi day activities FIRST for "Events, Speeches & Releases" and LAST for "In the News"

            foreach (ActiveActivity activity in activityList)
            {
                if (activity.StartDateTime >= reportEndDate) continue;
                if (BelongsToAwarenessConsultationReport(activity, outlookDate) != null) continue;

                string materials = activity.CommunicationsMaterials;
                var hqSection = (LookAheadSection)Math.Abs(activity.HqSection);

                bool isForIssuesSection = isAppOwner ? hqSection == LookAheadSection.Issues_and_Reports
                                                             : !activity.NotForLookAhead && !activity.IsConfirmed && (activity.EndDateTime - activity.StartDateTime).Value.Days >= 5;

                if (!reportDate.HasValue)
                { // Issues & Reports
                    if (!isForIssuesSection) continue;
                }
                else
                {
                    if (activity.EndDateTime < reportDate || isForIssuesSection) continue;
                    bool activityIsIntheNews = hqSection == LookAheadSection.In_the_News;
                    if (inTheNews)
                    {
                        if (!activityIsIntheNews) continue;
                    }
                    else if (hqSection != LookAheadSection.Events_and_Speeches)
                    {
                        continue;
                    }
                }

                var startDate = activity.StartDateTime.Value.Date;
                bool timeTBD = ActivityListProvider.IsTimeTBD(activity.StartDateTime.Value, activity.EndDateTime.Value, activity.IsConfirmed);
                string friendlyDate = ActivityListProvider.FriendlyDateTimeRange(activity, timeTBD, false, false, reportDate);


                var datePattern = @"\w{3}\s\w{3}\s\d{2}\s(0?[1-9]|1[0-2]):[0-5][0-9]\s(AM|PM)"; // check against "Thu May 27 6:10 PM" format
                var reformatFriendlyDate = Regex.IsMatch(friendlyDate, datePattern)
                    && (activity.HqStatus == "New" || activity.HqStatus == "Changed"); // formatting bug only occurs with new/changed

                if (reformatFriendlyDate)
                {
                    var capturePattern = @"\w{3}\s\w{3}\s(\d{2})\s(0?[1-9]|1[0-2]):[0-5][0-9]\s(AM|PM)"; // capture the numeric day value
                    var match = Regex.Match(friendlyDate, capturePattern);
                    if (match.Success)
                    {
                        var day = match.Groups[1].Value;                    
                        if(!string.IsNullOrWhiteSpace(day))
                            friendlyDate = friendlyDate.Replace(day, $"{day}<br />"); // fix report date alignment with an extra br tag
                    }
                }

                int? insertPos = null;
                if (reportDate.HasValue)
                {
                    if (timeTBD)
                    {
                        insertPos = inTheNews ? 0 : posOfFirstBlock;
                        posOfFirstBlock++;
                    }
                    else
                    {
                        bool isSameDay = startDate == activity.EndDateTime.Value.Date;
                        if (inTheNews == isSameDay)
                            insertPos = posOfFirstBlock++;
                    }
                }

                string titleDetails = FormatHqComments(activity, isAppOwner) ?? FormatTitleDetails(activity, false);
                titleDetails += FormatInitiative(activity, initiativesActivityIds);

                if (isDetailedLookAheadReport == true
                    && (hqSection == LookAheadSection.Events_and_Speeches
                        || hqSection == LookAheadSection.Issues_and_Reports || hqSection == LookAheadSection.In_the_News))
                {
                    titleDetails = string.Empty; // not ideal, but best to leave the existing code untouched for this scenario

                    string detailsSummary = !string.IsNullOrWhiteSpace(activity.Details) ? $"<br/>{activity.Details}" : "";

                    string cityVenueSeparator = !string.IsNullOrWhiteSpace(activity.City)
                                                    && !string.IsNullOrWhiteSpace(activity.Venue) ? ": " : "";

                    string city = !string.IsNullOrWhiteSpace(activity.City) && activity.City.Contains(", BC")
                        ? activity.City.Substring(0, activity.City.IndexOf(','))
                        : string.IsNullOrWhiteSpace(activity.City) ? "" : activity.City;

                    string cityVenue = string.IsNullOrWhiteSpace(activity.City)
                                                && string.IsNullOrWhiteSpace(activity.Venue) ? "" : $"<strong>{city}{cityVenueSeparator}{activity.Venue}</strong>";

                    string significance = !string.IsNullOrWhiteSpace(activity.Significance) ? $"<p>{activity.Significance}</p>" : "";

                    string lastUpdated = "";
                    if (activity.LastUpdatedDateTime.HasValue) {
                        var time = ActivityListProvider.FriendlyTime(activity.LastUpdatedDateTime.Value, true);

                        bool lastUpdatedYesterdayOrToday = activity.LastUpdatedDateTime.Value.Date == DateTime.Today 
                            || activity.LastUpdatedDateTime.Value.Date == DateTime.Today.AddDays(-1);

                        var when = "";
                        if (lastUpdatedYesterdayOrToday) 
                        {
                            if (activity.LastUpdatedDateTime.Value.Date == DateTime.Today)
                            {
                                when = "today";
                            } 
                            else if (activity.LastUpdatedDateTime.Value.Date == DateTime.Today.AddDays(-1)) 
                            {
                                when = "yesterday";
                            }
                        }

                        lastUpdated = lastUpdatedYesterdayOrToday 
                            ? $"&nbsp;&nbsp;<span style=\"font-size: 9pt; color:#CF7A50;\">Last updated {when} at {time}</span>"
                            : $"&nbsp;&nbsp;<span style=\"font-size: 9pt; color:#CF7A50;\">Last updated {ActivityListProvider.GetCreatedOrUpdatedMessage(activity.Status == "New", activity.CreatedDateTime, "", activity.LastUpdatedDateTime)}</span>";
                    }

                    titleDetails = $"<strong>{activity.Title}</strong>{detailsSummary} {significance} {cityVenue} {lastUpdated}";    
                }

                DataRow row = AddNewDetailsRow(titleDetailsColumn, titleDetails, insertPos);
                row[dateColumn] = friendlyDate;
                row[activityIdColumn] = activity.Id;
                row[ministryColumn] = activity.Ministry.Replace("GCPEHQ", "HQ");
                if (!reportDate.HasValue || startDate == reportDate.Value)
                    row[flagColumn] = FormatFlag(activity);

                if (categoryColumn != null)
                {
                    string category = activity.Categories;
                    if (activity.IsIssue)
                    {
                        category = "Issue";
                    }
                    else if (isAppOwner && category != " TV / Radio")
                    {
                        category = "FYI";
                    }
                    row[categoryColumn] = category;
                }

                if (activity.IsIssue && inTheNews)
                    row[colorColumn] = IssueColor; // Purple

                if(isDetailedLookAheadReport.Value && activity.IsIssue) row[colorColumn] = IssueColor; // highlight upcoming issues in the exec LA report

                //if (!isAppOwner || reportDate.HasValue) {
                row[releaseColumn] = FormatLookAheadRelease(inTheNews, activity, reportDate, materials);
                //else row[releaseColumn] = "TBD"; // Issues & Reports
            }
            //return numInTheNews;
        }

        public static DataRow AddNewRow(DataColumn column, object text, int? insertPos = null)
        {
            var table = column.Table;
            DataRow row = table.NewRow();
            row[column] = text;
            table.Rows.InsertAt(row, insertPos ?? table.Rows.Count);
            return row;
        }

        public static DataRow AddNewDetailsRow(DataColumn titleDetailsColumn, string titleDetails, int? insertPos)
        {
            // Parse and convert hyperlinks in details
            bool isHttp;
            string detailsEnd, url = ActivityListProvider.ParseDetailsForUrl(ref titleDetails, out detailsEnd, out isHttp);
            if (!string.IsNullOrEmpty(url))
            {
                string anchorText = url; // using Markdown convention [ ]
                int postStart = titleDetails.EndsWith("]", StringComparison.Ordinal) ? titleDetails.IndexOf('[') : -1;
                if (postStart != -1)
                {
                    anchorText = titleDetails.Substring(postStart + 1, titleDetails.Length - postStart - 2);
                    titleDetails = titleDetails.Substring(0, postStart);
                }
                else if (isHttp)
                { // no text => prettify a bit the url
                    anchorText = url.Substring("http://".Length);
                }
                titleDetails += "<a href='" + url + "'>" + anchorText + "</a>" + detailsEnd;
            }

            return AddNewRow(titleDetailsColumn, titleDetails, insertPos);
        }

        private static string FormatHqComments(ActiveActivity activity, bool isAppOwner)
        {
            string hqComments = isAppOwner ? activity.HqComments : "";
            if (hqComments == null || hqComments.Length <= 2) return null; // 2 for **

            while (true)
            {
                string marker = "**";
                int startPos = hqComments.IndexOf(marker, StringComparison.Ordinal);
                if (startPos == -1)
                {
                    marker = "_";
                    startPos = hqComments.IndexOf(marker, StringComparison.Ordinal);
                }
                if (startPos == -1) break;
                int endPos = hqComments.IndexOf(marker, startPos + marker.Length, StringComparison.Ordinal);
                if (endPos == -1) break;

                string toMarkdown = hqComments.Substring(startPos + marker.Length, endPos - (startPos + marker.Length));
                string htmlTag = marker == "_" ? "i>" : "b>";
                toMarkdown = "<" + htmlTag + toMarkdown + "</" + htmlTag;

                hqComments = hqComments.Substring(0, startPos) + toMarkdown + hqComments.Substring(endPos + marker.Length);
            }
            return hqComments.Replace("\r\n", "<br>");
        }

        private static string FormatTitleDetails(ActiveActivity activity, bool is30_60_90)
        {
            string titleDetails = (activity.NotForLookAhead ? "<span style='color:darkred'>Not for Look Ahead </span>" : string.Empty) + activity.Details;

            if (is30_60_90)
            {
                if (!string.IsNullOrEmpty(activity.Significance))
                {
                    titleDetails += "<br ><i>Significance: </i>" + activity.Significance;
                }
                titleDetails = "<span style='font-size:9pt'>" + titleDetails + "</span>";
            }
            titleDetails = "<b>" + FormatTitle(activity) + "</b>: " + titleDetails;
            return titleDetails;
        }

        private static string FormatTitle(ActiveActivity activity)
        {
            string city = FormatCity(activity);
            city = city == null ? "" : city + " - ";

            return city + activity.Title;
        }

        private static string FormatCity(ActiveActivity activity)
        {
            if (activity.City == null || activity.City == "TBD") return null;
            return activity.CityOrOther.Replace(", BC", "");
        }

        private static string FormatInitiative(ActiveActivity activity, IDictionary<int, string> initiativesActivityIds)
        {
            string initiative = null;
            if (initiativesActivityIds.TryGetValue(activity.Id, out initiative))
            {
                initiative = "<span style='color:seagreen'> " + initiative + "</span>";
            }
            return initiative;
        }
        private static string FormatFlag(ActiveActivity activity)
        {
            if (string.IsNullOrEmpty(activity.HqStatus)) return null;

            return "<br /><span style='color:orange'>" + activity.HqStatus.ToUpper() + "</span>";
        }

        private static string FormatLookAheadRelease(bool inTheNews, ActiveActivity activity, DateTime? reportDate, string materials)
        {
            string origins = activity.NROrigins;
            if (materials != null)
            {
                DateTime? nrDateTime = activity.NRDateTime;
                if (materials.Contains("News Release"))
                {
                    materials = "NR";
                }
                else if (materials.Contains("Information Bulletin"))
                {
                    materials = "IB";
                }
                else if (materials.Contains("Opinion Editorial"))
                {
                    materials = "OpEd";
                }
                else if (inTheNews && materials.Contains("Report"))
                {
                    materials = "Report";
                }
                else if (materials.Contains("Statement"))
                {
                    materials = "STMT";
                }
                else if (materials.Contains("Traffic Advisory"))
                {
                    materials = "TA";
                }
                else if (materials.Contains("News You Can Use"))
                {
                    materials = "NYCU";
                }
                else if (materials.Contains("Fact Sheet"))
                {
                    materials = "Fact<br >Sheet";
                }
                else
                {
                    nrDateTime = null;
                    if (inTheNews && materials.Contains("Newsletter"))
                    {
                        materials = "e-news";
                    }
                    else
                    {
                        materials = null;
                    }
                }

                if (nrDateTime.HasValue && nrDateTime != activity.StartDateTime && nrDateTime.Value.Date == reportDate)
                {
                    materials += "<br >" + nrDateTime.Value.ToString("h:mm tt").ToLower();
                }
            }
            if (origins != null)
            {
                string release = null;
                if (origins.Contains("Ministry"))
                {
                    release = "BCGov";
                }
                else if (origins.Contains("Joint"))
                {
                    release = "Joint";
                }
                else if (origins.Contains("3rd party"))
                {
                    release = "3rd party";
                }
                if (release != null)
                {
                    materials = materials == null ? release : release + "<br >" + materials;
                }
            }
            return string.IsNullOrEmpty(materials) ? "-" : materials;
        }
        private void ClearLAStatus()
        {
            int numberDays;
            if (Int32.TryParse(request.QueryString["numberDays"], out numberDays))
            {
                DateTime today = DateTime.Today;
                DateTime endDate = today.AddDays(numberDays);
                using (var dc = new CorporateCalendarDataContext())
                {
                    foreach (var activity in dc.Activities.Where(a => a.StartDateTime <= endDate && a.HqStatusId != null).ToList())
                    {
                        activity.HqStatusId = null;
                    }
                    dc.SubmitChanges();
                }
            }
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