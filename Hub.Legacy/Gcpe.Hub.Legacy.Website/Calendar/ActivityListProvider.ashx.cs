using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using System.Web;
using System.Web.Script.Serialization;
using CorporateCalendar.Data;
using CorporateCalendar.Security;

namespace Gcpe.Hub.Calendar
{
    /// <summary>
    /// This is used by default.aspx to retrieve the activities
    /// </summary>
    public class ActivityListProvider : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string recordsPerPage = context.Request.Form["rp"];
            string pageIndex = context.Request.Form["page"];
            string sortColumn = context.Request.Form["sortname"];
            string sortOrder = context.Request.Form["sortorder"];

            int iRecordsPerPage = 10;
            int.TryParse(recordsPerPage, out iRecordsPerPage);
            int iPageIndex = 1;
            int.TryParse(pageIndex, out iPageIndex);

            int startIndex = (iPageIndex - 1) * iRecordsPerPage;

            // Since this class does not inherit the masterpage, we must construct an instance of the current user's CustomPrincipal from the session.
            var customPrincipal = new CustomPrincipal(HttpContext.Current.User.Identity);

            bool includeMarkup = false;

            //-- Only for Administrator + System Administrator we want to include the mark-up
            //-- If include review mark-up is true, we want to add a CSS style class
            //-- Otherwise add no mark-up
            if (customPrincipal.IsGCPEHQ && customPrincipal.IsInRoleOrGreater(SecurityRole.Administrator))
            {
                includeMarkup = true;
            }

            List<ActiveActivity> activeActivitiesForGridView;
            IList<string> hiddenColumns = null;

            if (string.IsNullOrEmpty(context.Request.QueryString["name"])
                && !string.IsNullOrEmpty(context.Request.QueryString["op"])
                && context.Request.QueryString["op"] == "rcc") // user is trying to run a "corporate query" - rcc stands for Run Corporate Query
            {
                activeActivitiesForGridView = GetCorporateQueryActivities(customPrincipal, context.Request);
            }
            else // user is trying to run a "my query" or they are running a filter at run time
            {
                int display = int.TryParse(context.Request["display"], out display) ? display : 3;

                using (var dc = new CorporateCalendarDataContext())
                {
                    var systemUser = dc.SystemUsers.SingleOrDefault(su => su.Id == customPrincipal.Id);
                    if (systemUser != null)
                    {
                        hiddenColumns = (systemUser.HiddenColumns?.Split(',') ?? ColumnModel.HiddenByDefault).ToList();
                        string column2toggle = context.Request["cid2toggle"];
                        if (systemUser.FilterDisplayValue != display || !string.IsNullOrEmpty(column2toggle))
                        {
                            systemUser.FilterDisplayValue = display;
                            if (!string.IsNullOrEmpty(column2toggle))
                            {
                                if (!hiddenColumns.Remove(column2toggle))
                                {
                                    hiddenColumns.Add(column2toggle);
                                }
                                systemUser.HiddenColumns = string.Join(",", hiddenColumns.OrderBy(c => c));
                            }
                            dc.SubmitChanges();
                        }
                    }
                }

                bool hasFilters = context.Request.QueryString.Count > 0;

                string category = context.Request["category"] ?? (hasFilters ? null : "-2"); // Not Awareness dates
                string ministry = context.Request["ministry"] ?? (hasFilters || customPrincipal.SingleMinistryName == "CITENG" ? null : "CITENG"); // Not Citizen Engagement

                activeActivitiesForGridView = GetFilteredActivities(customPrincipal, context.Request, display, category, ministry, includeMarkup);
            }
            activeActivitiesForGridView = Sort(activeActivitiesForGridView, sortColumn, sortOrder).ToList();

            int totalRecords = activeActivitiesForGridView.Count;
            int numRecordsAfterIndex = totalRecords - startIndex;
            if (numRecordsAfterIndex <= 0)
            { // the user ran another query that has less records
                startIndex = 0;
                iPageIndex = 1;
                numRecordsAfterIndex = totalRecords - startIndex;
            }

            if (iRecordsPerPage > numRecordsAfterIndex)
                iRecordsPerPage = numRecordsAfterIndex;

            string lastChangeTime = activeActivitiesForGridView.Any() ? activeActivitiesForGridView.Max(a => a.LastUpdatedDateTime ?? a.CreatedDateTime ?? DateTime.MinValue).ToString("yyyy-MM-ddTHH:mm:ss.fffffff") : "";

            List<UIDataRow> dataRows = GetUIDataRowsFromList(activeActivitiesForGridView.GetRange(startIndex, iRecordsPerPage), customPrincipal.Id, includeMarkup, hiddenColumns);

            UIDataModel uiDataModel = new UIDataModel();
            uiDataModel.rows = new List<UIDataRow>();
            uiDataModel.page = iPageIndex;
            uiDataModel.total = totalRecords;
            uiDataModel.loadedTime = lastChangeTime;
            uiDataModel.rows.AddRange(dataRows);

            string jsonString = new JavaScriptSerializer().Serialize(uiDataModel);
            WriteToResponse(jsonString, context.Response);
        }

        internal static IEnumerable<ActiveActivity> Sort(List<ActiveActivity> activities, string sortColumn, string sortOrder = "asc")
        {
            string sortColumn2 = null;
            if (sortColumn == "TitleDetails")
                sortColumn = "Title";
            else if (sortColumn == "NameAndNumber")
                sortColumn = "ContactName";
            else if (sortColumn == "StartEndDateTime")
            {
                sortColumn = "StartDateTime";
                sortColumn2 = "EndDateTime";
            }
            else if (sortColumn == "MinistryActivityId")
            {
                sortColumn = "Ministry";
                sortColumn2 = "Id";
            }
            else if (sortColumn == "Keywords")
            {
                sortColumn = "ActivityKeywords";
            }
            return ListSorter.Sort(activities, "asc".Equals(sortOrder), sortColumn, sortColumn2 ?? "StartDateTime");
        }

        internal static int? GetIntParam(string key, HttpRequest request)
        {
            string param = request[key];

            return param == null ? null : (int?)Convert.ToInt32(param);
        }

        /// <summary>
        /// User is trying to run a "my query" 
        /// OR 
        /// they are running a filter at run time
        /// </summary>
        /// <param name="customPrincipal"></param>
        /// <param name="filterDictionary"></param>
        /// <param name="display"></param>
        /// <param name="categoryId"></param>
        /// <param name="ministryId"></param>
        /// <param name="includeMarkup"></param>
        /// <returns></returns>
        public static List<CorporateCalendar.Data.ActiveActivity> GetFilteredActivities(CustomPrincipal customPrincipal,
            HttpRequest request,
            int display,
            string category = null,
            string ministry = null,
            bool includeMarkup = false)
        {
            var systemUserMinistries = customPrincipal.SystemUserMinistryIds; // Get the list of user's ministries
            bool isCurrentUserInOwnerList = customPrincipal.IsInApplicationOwnerOrganizations;

            int? contactId = GetIntParam("contact", request);
            if (display == 4)  // 4 is my or specific com contact's activities.
            {
                display = 2;

                // if there isn't a contact in the filter then add current user
                if (contactId == null)
                    contactId = customPrincipal.Id;
            }

            bool? excludeshared = display == 2 ? true : (bool?)null;
            bool? keyActivities = display == 4 ? true : (bool?)null;
            bool? myFavorites = display == 10 ? true : (bool?)null;

            bool myBool;
            // The grid should initially only display activities from today onward
            List<ActiveActivity> queryResults = ActivityDAO.GetActivitiesSecurely(customPrincipal.Id,
                isCurrentUserInOwnerList ? null : systemUserMinistries,
                GetIntParam("status", request),
                (request["datefrom"] != null ? Convert.ToDateTime(request["datefrom"]) : DateTime.Today),
                (request["dateto"] != null ? Convert.ToDateTime(request["dateto"]) : (DateTime?)null),
                (category != null ? Convert.ToInt32(category) : (int?)null), ministry,
                contactId,
                GetIntParam("representative", request),
                GetIntParam("premierRequested", request),
                GetIntParam("initiative", request),
                request["keywords"]?.Split('~'),
                bool.TryParse(request["dateConfirmed"], out myBool) ? myBool : (bool?)null,
                bool.TryParse(request["isissue"], out myBool) ? myBool : (bool?)null,
                bool.TryParse(request["thisdayonly"], out myBool) ? myBool : (bool?)null,
                request["quickSearch"]?.ToLower(), includeMarkup, excludeshared, keyActivities, myFavorites).ToList();

            var filteredList = new List<CorporateCalendar.Data.ActiveActivity>();
            bool isEditorOrReadOnly = customPrincipal.IsInRole(SecurityRole.ReadOnly) ||
               customPrincipal.IsInRole(SecurityRole.Editor);
            string lookahead = request["lookahead"];

            foreach (var item in queryResults)
            {
                if (lookahead != null)
                {
                    if (Convert.ToBoolean(lookahead))
                    {
                        if (!item.IsConfidential)
                            filteredList.Add(item);
                    }
                    else
                    {
                        if (item.IsConfidential)
                            filteredList.Add(item);
                    }
                }
                else if (isCurrentUserInOwnerList && isEditorOrReadOnly)
                {
                    string[] sharedMinistriedIDs = item.SharedWithMinistryIds == null ? new string[] { } : item.SharedWithMinistryIds.Split(',');
                    bool isSharedActivity = sharedMinistriedIDs.Any(s => systemUserMinistries.Contains(Guid.Parse(s)));
                    if ((item.ContactMinistryId.HasValue && systemUserMinistries.Contains(item.ContactMinistryId.Value)) || !item.IsConfidential || isSharedActivity)
                        filteredList.Add(item);
                }
                else
                {
                    filteredList.Add(item);
                }

                //if (item.IsStartDateNeedsReview && item.IsEndDateNeedsReview)
                //{
                //    item.StartEndDateTime += string.Format("<div style=\"padding:0px;margin:1px 0px 0px 0px;color:#404040\">{0}</div>", item.Schedule);
                //}
            }

            return filteredList;
        }

        private List<ActiveActivity> GetCorporateQueryActivities(CustomPrincipal customPrincipal, HttpRequest request)
        {
            var systemUserMinistries = customPrincipal.SystemUserMinistryIds; // Get the list of user's ministries
            List<ActiveActivity> queryResults;

            // Passed in from query strings
            bool showAll;
            bool.TryParse(request["showAll"], out showAll);
            int nbrDays = 0;
            int.TryParse(request["nbrDays"], out nbrDays);
            bool isCurrentUserInOwnerList = customPrincipal.IsInApplicationOwnerOrganizations;

            // Used for Queries
            string[] statuses = request["statuses"].Split(',');
            string[] hqStatuses = request["hqStatuses"].Split(',');
            bool includeDeleted;
            bool.TryParse(request["deletedYN"], out includeDeleted);
            DateTime? endDatetime = null;
            if (!showAll)
                endDatetime = DateTime.Today.AddDays(nbrDays + 1);

            queryResults = ActivityDAO.GetAllActivitiesSecurely(customPrincipal.Id,
                            isCurrentUserInOwnerList,
                            endDatetime,
                            statuses,
                            hqStatuses,
                            includeDeleted);

            var filteredList = new List<CorporateCalendar.Data.ActiveActivity>();
            bool isEditorOrReadOnly = customPrincipal.IsInRole(SecurityRole.ReadOnly) ||
               customPrincipal.IsInRole(SecurityRole.Editor);

            string lookahead = request["lookahead"];
            foreach (var item in queryResults)
            {
                if (lookahead != null)
                {
                    if (Convert.ToBoolean(lookahead))
                    {
                        if (!item.IsConfidential)
                            filteredList.Add(item);
                    }
                    else
                    {
                        if (item.IsConfidential)
                            filteredList.Add(item);
                    }
                }
                else if (isCurrentUserInOwnerList && isEditorOrReadOnly)
                {
                    string[] sharedMinistriedIDs = item.SharedWithMinistryIds == null ? new string[] { } : item.SharedWithMinistryIds.Split(',');
                    bool isSharedActivity = sharedMinistriedIDs.Any(s => systemUserMinistries.Contains(Guid.Parse(s)));
                    if ((item.ContactMinistryId.HasValue && systemUserMinistries.Contains(item.ContactMinistryId.Value)) || item.IsConfidential != true || isSharedActivity)
                        filteredList.Add(item);
                }
                else
                {
                    filteredList.Add(item);
                }
            }

            /*foreach (var activity in filteredList)
            {
                string startDateTimeString = "<strong>Start: </strong> " + ((DateTime)activity.StartDateTime).ToString("MMM d yyyy h:mm tt");
                string endDateTimeString = "<strong>End: </strong> " + ((DateTime)activity.EndDateTime).ToString("MMM d yyyy h:mm tt");
                activity.StartEndDateTime = "<div style=\"text-align: left; width: 140px; white-space: normal; padding: 0px;\" title=\"" + activity.Schedule + "\">" + startDateTimeString + "<br />" + endDateTimeString + "</div>";

                if (activity.IsStartDateNeedsReview && activity.IsEndDateNeedsReview)
                {
                    activity.StartEndDateTime += "<br />" + activity.Schedule;
                }
            }*/

            return filteredList;
        }


        /// <summary>
        /// Writes to response.
        /// </summary>
        /// <param name="jsonString">The json string.</param>
        private void WriteToResponse(string jsonString, HttpResponse response)
        {
            response.AddHeader("Cache-Control", "no-cache , must-revalidate");
            response.AddHeader("Pragma", "no-cache");
            response.Clear();
            response.ContentType = "application/json";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(jsonString);
            response.AddHeader("Content-Length", buffer.Length.ToString());
            response.BinaryWrite(buffer);
            response.Flush();
            response.Close();
        }

        /* Remove_RecordLocks
         * protected List<UIDataRow> GetUIDataRowsFromList(List<CorporateCalendar.Data.ActiveActivity> list, IEnumerable<CorporateCalendar.Data.RecordLock> RecordLocks)
        */

        protected string ApplyMarkup(Func<string, string> markup, string text)
        {
            return markup != null ? markup(text) : text;
        }

        /// <summary>
        /// Formats the results for the grid...
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">The list.</param>
        /// <returns></returns>
        protected List<UIDataRow> GetUIDataRowsFromList(List<ActiveActivity> list, int systemUserId, bool includeReviewMarkup, IList<string> hiddenColumns)
        {
            var dataRows = new List<UIDataRow>();
            using (var dc = new CorporateCalendar.Data.CorporateCalendarDataContext())
            {
                var users = dc.SystemUsers.ToArray();


                // Only for Administrator + System Administrator
                // If include review markup is true, we want to add a css style class
                // Otherwise add no markup
                var Markup = includeReviewMarkup ? new Func<string, string>((s) => "<span class='reviewed'>" + s + "</span>") : null;

                foreach (var t in list)
                {
                    var dr = new UIDataRow();
                    dr.cell = new Hashtable();
                    /*if (t.IsPremierRequestedOrConfirmed)
                        dr.color = "lightpink";
                    else if (t.IsIssue)
                        dr.color = "#ccc0d9"; */ // look ahead purple

                    string ministryAbbreviation = t.Ministry;
                    string activityId = t.Id.ToString();
                    bool isReviewed = t.StatusId == 2;
                    bool isNew = t.StatusId == 7;
                    //bool isLockedForEditing = false;
                    bool isActive = t.IsActive;
                    bool IsActiveNeedsReview = t.IsActiveNeedsReview;
                    bool isSharedWithMinistries = !string.IsNullOrEmpty(t.SharedWithMinistryIds);
                    string favoriteImageTag = string.Empty;
                    string isReviewedImageTag = string.Empty;
                    string isInternalImageTag = string.Empty;
                    string isLockedImageTag = string.Empty;
                    string isSharedImageTag = string.Empty;
                    //string isConfirmedImageTag = string.Empty;

                    string details = t.Details == "" ? t.Comments : t.Details;
                    bool isHttp;
                    string detailsEnd, url = ParseDetailsForUrl(ref details, out detailsEnd, out isHttp);
                    if (!string.IsNullOrEmpty(url))
                    {
                        details += CorporateCalendar.Utility.GridHelper.Linkify(isHttp ? url.Substring("http://".Length) : url, url) + detailsEnd;
                    }
                    string significance = t.Significance;

                    details = ApplyMarkup(t.IsDetailsNeedsReview ? Markup : null, details);
                    if (t.NotForLookAhead)
                        details = "<span style='color:darkred'>Not for Look Ahead</span> " + details;

                    string city = ApplyMarkup(t.IsCityNeedsReview ? Markup : null, t.CityOrOther);
                    string venue = t.Venue;
                    string otherCity = t.OtherCity;
                    string schedulingConsiderations = t.Schedule;
                    string changedMessage = "";

                    string categories = (t.Categories ?? "").Replace("&", "&amp;").Trim();

                    if (t.IsIssue)
                    {
                        categories = "Issue, " + categories;

                        categories = categories.Replace(", FYI Only", "");
                    }

                    //if (t.IsConfidential)
                    //{
                    //    categories = "<span style='color:darkred'>Restricted</span>, " + categories;
                    //}

                    categories = categories.TrimEnd(',', ' ');

                    dr.cell.Add("Categories", ApplyMarkup(t.IsCategoriesNeedsReview ? Markup : null, categories));

                    if (!ColumnModel.IsHidden("Keywords", hiddenColumns))
                    {
                        dr.cell.Add("Keywords", t.Keywords ?? "");
                    }
                    if (!ColumnModel.IsHidden("Ministry", hiddenColumns))
                    {
                        dr.cell.Add("Ministry", t.Ministry);
                    }
                    if (!ColumnModel.IsHidden("Status", hiddenColumns))
                    {
                        string status = ApplyMarkup(t.StatusId != 2 || t.IsActiveNeedsReview ? Markup : null, t.IsActive ? t.Status : "Deleted");
                        dr.cell.Add("Status", status + (string.IsNullOrEmpty(t.HqStatus) ? "" : "<br/><span style='font-size:11px'>LA&nbsp;" + t.HqStatus + "<span>"));
                    }
                    if (!ColumnModel.IsHidden("Translations", hiddenColumns))
                    {
                        dr.cell.Add("Translations", t.Translations ?? "");
                    }
                    if (!ColumnModel.IsHidden("PremierRequested", hiddenColumns))
                    {
                        dr.cell.Add("PremierRequested", t.PremierRequested == "Yes" ? "Premier Reqstd" : t.PremierRequested ?? "");
                    }
                    if (!ColumnModel.IsHidden("GovernmentRepresentative", hiddenColumns))
                    {
                        string representatives = ApplyMarkup(t.IsRepresentativeNeedsReview ? Markup : null, (t.GovernmentRepresentative ?? ""));
                        if (t.IsPremierRequestedOrConfirmed)
                        {
                            representatives = "Premier" + (representatives == "" ? "" : ", ") + representatives;
                        }
                        dr.cell.Add("GovernmentRepresentative", representatives);
                    }

                    if (!string.IsNullOrEmpty(t.FavoriteSystemUsers))
                    {
                        var favoriteUserIds = t.FavoriteSystemUsers.Split(',').Where(e => e != string.Empty);

                        string favoriteUserNames = string.Join(", ", users.Where(e => favoriteUserIds.Contains(e.Id.ToString())).Select(e => e.FullName).OrderBy(e => e));
                        string icon = favoriteUserIds.Contains(systemUserId.ToString()) ? "../images/icon-star.png" : "../images/icon-star-grey.png";

                        favoriteImageTag = "<img height='16' style='float:left;' title='" + favoriteUserNames.Replace("'", "''") + "' src='" + icon + "' />";
                    }

                    //For Active and on reviewed status activity, or deleted activity and its IsActiveNeedsReview flag is false, display reviewed image
                    if ((isReviewed && isActive) || (!isActive && !IsActiveNeedsReview))
                    {
                        isReviewedImageTag =
                            "<img style='float:left' title='This activity has been reviewed' src='../images/eye-pencil-icon.png' />";
                    }

                    string title = ApplyMarkup(t.IsTitleNeedsReview ? Markup : null, t.Title);

                    string linkifiedTitle = CorporateCalendar.Utility.GridHelper.Linkify(title,
                            "Activity.aspx?ActivityId=" +
                            activityId);

                    string titleAndDetailWithFormating = "";
                    if (!string.IsNullOrEmpty(significance))
                    {// use span instead of div otherwise column resize breaks in flexigrid
                        titleAndDetailWithFormating += "<span title=\"" + significance + "\">";
                    }

                    titleAndDetailWithFormating +=
                        string.Format("<span style='display:block; padding:0px; margin-bottom: 2px;'>{0}</span>", linkifiedTitle);

                    if (details.Trim().Length > 0)
                    {
                        titleAndDetailWithFormating += string.Format("<span style='padding:0px;margin:1px 0px 0px 0px;color:#404040'>{0}</span>", details);
                    }

                    if (!string.IsNullOrEmpty(significance))
                    {
                        titleAndDetailWithFormating += "</span>";
                    }

                    dr.cell.Add("Id", t.Id);
                    dr.cell.Add("LeadOrganization", t.LeadOrganization);
                    dr.cell.Add("NameAndNumber", t.ContactName + "<br/>" + t.ContactNumber.Replace("-", "\u2011")); //\u2011 = non-breaking hyphen

                    string startEndHoverText = string.Empty;
                    bool timeTBD = false;
                    if (!t.IsConfirmed && !string.IsNullOrEmpty(t.PotentialDates))
                    {
                        startEndHoverText = "Date & Time: " + FriendlyDateTimeRange(t.StartDateTime.Value, t.EndDateTime.Value, t.IsAllDay, false);
                        timeTBD = IsTimeTBD(t.StartDateTime.Value, t.EndDateTime.Value, t.IsConfirmed);
                        startEndHoverText += timeTBD ? " Time TBD\r\n" : " TBC\r\n";
                    }
                    startEndHoverText += "Considerations: " + t.Schedule;

                    string startEndDateTime = FriendlyDateTimeRange(t, timeTBD, true);
                    bool isMultiYear = t.StartDateTime.Value.Year != t.EndDateTime.Value.Year;
                    if (isMultiYear) startEndDateTime = startEndDateTime.Replace("-", "-<br />"); // Forces Firefox to break

                    if (t.IsStartDateNeedsReview || t.IsEndDateNeedsReview)
                    {
                        string[] dateTimes = startEndDateTime.Split('-');

                        dateTimes[0] = ApplyMarkup(t.IsStartDateNeedsReview ? Markup : null, dateTimes[0]);
                        if (dateTimes.Length > 1)
                        {
                            dateTimes[1] = ApplyMarkup(t.IsEndDateNeedsReview ? Markup : null, dateTimes[1]);
                        }
                        startEndDateTime = string.Join("-", dateTimes);
                    }

                    dr.cell.Add("StartEndDateTime", "<span style='text-align: left; width: 140px; white-space: normal; padding:0px;' title='" + startEndHoverText + "'>" + startEndDateTime);
                    dr.cell.Add("start", t.StartDateTime.Value.ToString("s"));
                    dr.cell.Add("end", t.EndDateTime.Value.ToString("s"));
                    dr.cell.Add("title", string.Format("<span style='color:#034af3'>{0}</span>", title)); // color of a link
                    dr.cell.Add("details", details);

                    dr.cell.Add("CreatedDateTime", (t.CreatedDateTime ?? DateTime.MinValue).ToOADate().ToString());
                    dr.cell.Add("LastUpdatedDateTime", (t.LastUpdatedDateTime ?? DateTime.MinValue).ToOADate().ToString());
                    dr.cell.Add("TitleDetails", titleAndDetailWithFormating);

                    dr.cell.Add("City", string.IsNullOrEmpty(venue) ? city : city + "<br />" + venue);

                    if (hiddenColumns?.Contains("7") != true)
                    {
                        string commMaterials = string.Empty;
                        if (t.CommunicationsMaterials != null)
                        {
                            commMaterials = ApplyMarkup(t.IsCommMaterialsNeedsReview ? Markup : null, t.CommunicationsMaterials.Replace("&", "&amp;"));
                        }
                        string origins = GetNROriginsString(t);
                        if (!string.IsNullOrEmpty(origins) && !string.IsNullOrEmpty(t.NRDistributions))
                        {
                            origins += "&#013;"; //<br/>
                        }

                        dr.cell.Add("CommunicationsMaterials", " <span title=\"" + origins + t.NRDistributions + "\">" + commMaterials + "</span>");
                    }

                    if (isSharedWithMinistries)
                    {
                        //isSharedWithOtherMinistries = true;
                        isSharedImageTag =
                            "<img style='float:left' title='This activity is shared with other ministries' src='../images/shared.png' />";
                    }

                    /*if (isConfirmed)
                    {
                        isConfirmedImageTag =
                            "<img style='float:left' title='This activity has its date confirmed' src='images/confirmed.png' />";
                    }
                    if (isInternal)
                    {
                        isInternalImageTag =
                            "<img style='float:left' title='This activity is flagged as internal' src='images/internal.png' />";
                    }
                    if (isLockedForEditing)
                    {
                        isLockedImageTag =
                            "<img style='float:left' title='This activity is locked for editing' src='images/locked.png' />";
                    }*/

                    changedMessage = GetCreatedOrUpdatedMessage(isNew, t.CreatedDateTime, changedMessage, t.LastUpdatedDateTime);
                    DateTime? relevantDate = t.LastUpdatedDateTime ?? t.CreatedDateTime;
                    string toolTipMessage = "";

                    if (relevantDate.HasValue)
                    {
                        toolTipMessage = FriendlyDateTime(relevantDate.Value);
                        toolTipMessage += " at " + relevantDate.Value.ToString("h:mm tt").ToLower();
                        int? relevantUserId = t.LastUpdatedBy ?? t.CreatedBy;
                        if (relevantUserId.HasValue)
                        {
                            string relevantUserName = users.Single(e => e.Id == relevantUserId).FullName;
                            toolTipMessage += "\r\nby " + relevantUserName;
                        }
                    }

                    dr.cell.Add("MinistryActivityId", string.Format("<span style='padding:0px;'>{0}{1}{2}{3}{4}{5}</span><br/><span style='padding-left:0px;padding-right:0px;color:#2c3539;' title='" + toolTipMessage + "'>{6}</span><br /><span style='font-size:11px; color: red;'>created {7}</span>",
                            favoriteImageTag, isReviewedImageTag, isInternalImageTag, isLockedImageTag, isSharedImageTag,
                            CorporateCalendar.Utility.GridHelper.Linkify(string.Format("{0}-\u200B{1}", // \u200B : 0-width space to make Firefox break lines at hyphens, like other browsers
                            ministryAbbreviation, activityId), "Activity.aspx?ActivityId=" + activityId), changedMessage, t.CreatedDateTime?.ToString("MMM d, yyyy")));

                    dataRows.Add(dr);
                }
            }

            return dataRows;
        }

        public static string GetCreatedOrUpdatedMessage(bool isNew, DateTime? createdDateTime, string message,
            DateTime? lastUpdatedDateTime)
        {
            DateTime? messageDate;
            if (isNew || lastUpdatedDateTime == null)
            {
                message += "created ";
                messageDate = createdDateTime;
            }
            else
            {
                message += "updated ";
                messageDate = lastUpdatedDateTime;

            }
            if (messageDate < DateTime.Today && messageDate >= DateTime.Today.AddDays(-1))
                message += "yesterday";
            else
                message += FriendlyTimeSpan(messageDate).ToLower() + " ago";
            return message;
        }

        ///// <summary>
        ///// Gets the UI data rows from data table.
        ///// </summary>
        ///// <param name="dt">The dt.</param>
        ///// <returns></returns>
        //protected List<UIDataRow> GetUIDataRowsFromDataTable(DataTable dt)
        //{
        //    List<UIDataRow> dataRows = new List<UIDataRow>();

        //    int idx = 1;
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        UIDataRow dr = new UIDataRow();
        //        dr.cell = new Hashtable();
        //        dr.rowid = idx++;

        //        foreach (DataColumn column in dt.Columns)
        //        {
        //            object o = row[column];
        //            if (!Convert.IsDBNull(o))
        //            {
        //                dr.cell.Add(column.ColumnName, o.ToString());
        //            }
        //            else
        //            {
        //                dr.cell.Add(column.ColumnName, string.Empty);
        //            }
        //        }
        //        dataRows.Add(dr);
        //    }

        //    return dataRows;
        //}


        //public class CalendarItem
        //{
        //    public string id { get; set; }
        //    public string title { get; set; }
        //    public string start { get; set; }
        //    public string end { get; set; }
        //    public bool allDay { get; set; }
        //}

        private string GetNROriginsString(ActiveActivity t)
        {
            var origins = "";
            if (t.NROrigins != null)
            {
                string[] NROrigins = t.NROrigins.Split(',');
                for (int i = 0; i < NROrigins.Length; i++)
                {
                    string origin = NROrigins[i];
                    if (i != 0)
                        origins += ",";
                    //Follow the convention used in Activity (JavaScript: UpdateOriginLabels())
                    //TODO: Rename options in the database to ensure both pages remain consistent.

                    origin = origin.Replace("Fed", "Federal").Replace("Federaleral", "Federal");

                    origin = origin.Replace("Ministry", "Ministry (" + t.Ministry + ")");

                    origin = origin.Replace("Prov", "Ministry (" + t.Ministry + ")");

                    if (!string.IsNullOrEmpty(t.LeadOrganization))
                        origin = origin.Replace("3rd party", t.LeadOrganization);

                    origin = origin.Replace("/", " / ");

                    origin = origin.Replace(" release", "") + " release";

                    origins += origin;
                }
            }
            return origins;
        }

        const char nbsp = (char)160;
        public static string FriendlyDateTimeRange(ActiveActivity activity, bool displayDayOfWeek)
        {
            bool timeTBD = IsTimeTBD(activity.StartDateTime.Value, activity.EndDateTime.Value, activity.IsConfirmed);
            return FriendlyDateTimeRange(activity, timeTBD, displayDayOfWeek);
        }

        public static bool IsTimeTBD(DateTime start, DateTime end, bool isConfirmed)
        {
            if (start.Date == end.Date)
            {
                return !isConfirmed && start.TimeOfDay == new TimeSpan(8, 0, 0) && end.TimeOfDay == new TimeSpan(18, 0, 0);
            }
            return false;
        }

        public static string FriendlyDateTimeRange(ActiveActivity activity, bool timeTBD, bool displayDayOfWeek, bool displayEndTime = true, DateTime? referenceDay = null) // Only the Look Ahead uses a reference day, defaults to Today otherwise
        {
            bool isConfirmed = activity.IsConfirmed;
            string value = activity.PotentialDates;
            if (isConfirmed || string.IsNullOrEmpty(value))
            {
                value = FriendlyDateTimeRange(activity.StartDateTime.Value, activity.EndDateTime.Value, activity.IsAllDay, !timeTBD, displayDayOfWeek, displayEndTime, referenceDay);
                if (isConfirmed)
                    return value;
            }

            return value + " <b><span style='color:#1919d2;'>" + (timeTBD ? "Time TBD" : "TBC") + "</span></b>";
        }

        public static string FriendlyDateTimeRange(DateTime start, DateTime end, bool isAllDay = true, bool displayTime = true, bool displayDayOfWeek = true, bool displayEndTime = true, DateTime? referenceDay = null)
        {
            DateTime startDate = start.Date, endDate = end.Date;
            if (startDate == endDate)
            {
                return FriendlyDateTime(start, displayEndTime ? (DateTime?)end : null, isAllDay, displayTime, displayDayOfWeek, referenceDay);
            }
            else
            {
                DateTime reference = referenceDay ?? DateTime.Today;
                bool displayYear = start.Year != end.Year || start.Year != reference.Year;
                string value = FriendlyDate(startDate, displayYear) + "-";
                bool displayMonth = displayYear || end.Month != start.Month;
                value += displayMonth ? FriendlyDate(endDate, displayYear) : string.Format("{0:%d}", endDate);
                return value;
            }
        }

        public static string FriendlyDateTime(DateTime start, DateTime? end = null, bool isAllDay = true, bool displayTime = true, bool displayDayOfWeek = true, DateTime? referenceDay = null)
        {
            string value;
            DateTime reference = referenceDay ?? DateTime.Today;
            DateTime startDate = start.Date;
            if (startDate == reference)
            {
                value = !referenceDay.HasValue || isAllDay ? FriendlyDate(startDate, false, "ddd" + nbsp + "MMM") : "";
            }
            else
            {
                value = FriendlyDate(startDate, start.Year != reference.Year, displayDayOfWeek ? "ddd" + nbsp + "MMM" : null);
            }

            if (displayTime && !isAllDay)
            {
                bool startsPm = start.Hour >= 12;
                value += " " + FriendlyTime(start, !end.HasValue || startsPm != (end.Value.Hour >= 12));
                if (end.HasValue)
                { // Not Look Ahead
                    value += "-" + FriendlyTime(end.Value, true);
                }
            }
            return value;
        }

        public static string FriendlyDate(DateTime date, bool displayYear, string format = null)
        {
            format = (format ?? "MMM") + nbsp + "d";
            return date.ToString(displayYear ? format + nbsp + "yyyy" : format);
        }

        public static string FriendlyTime(DateTime time, bool displayAmPm)
        {
            return displayAmPm ? time.ToString("h:mm" + nbsp + "tt") : time.ToString("h:mm");
        }

        public static string FriendlyTimeSpan(DateTime? time)
        {
            TimeSpan timeSpan = DateTime.Now.Subtract(time.Value);
            if (timeSpan.Days / 30 > 0)
            {
                return timeSpan.Days / 30 == 1 ? "1 Month" : timeSpan.Days / 30 + " Months";
            }
            else if (timeSpan.Days / 7 > 0)
            {
                return timeSpan.Days / 7 == 1 ? "1 Week" : timeSpan.Days / 7 + " Weeks";
            }
            else if (time.Value.Date == DateTime.Today)
            {
                if (timeSpan.Hours > 0)
                {
                    return timeSpan.Hours == 1 ? "1 Hour" : timeSpan.Hours + " Hours";
                }
                else
                {
                    return timeSpan.Minutes == 1 ? "1 Minute" : timeSpan.Minutes + " Minutes";
                }
            }
            else
            {
                return (DateTime.Today - time.Value.Date).TotalDays + " Days";
            }
        }
        static char[] urlTerminators = { ' ', '<', '\r' };
        public static string ParseDetailsForUrl(ref string details, out string detailsEnd, out bool isHttp)
        {
            isHttp = true;
            detailsEnd = "";
            int postStart = details.IndexOf("http://", StringComparison.Ordinal);
            if (postStart == -1)
            {
                isHttp = false;
                postStart = details.IndexOf("https://", StringComparison.Ordinal);
            }
            if (postStart == -1) return null;
            string url = details.Substring(postStart);

            int posEnd = url.IndexOfAny(urlTerminators);
            if (posEnd != -1)
            {
                detailsEnd = url.Substring(posEnd);
                url = url.Substring(0, posEnd);
            }
            if (url.Length <= "http://".Length) return null;
            details = details.Substring(0, postStart);
            return url.TrimEnd('/');
        }
        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
    }
}