using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using CorporateCalendar.Data;

/// <summary>
/// Summary description for ActivityDAO
/// </summary>
public class ActivityDAO
{
    public ActivityDAO() { /* Default Constructor */}


    /// <summary>
    /// 
    /// </summary>
    /// <param name="systemUserId"></param>
    /// <param name="isCurrentUserInOwnerList"></param>
    /// <param name="statusId"></param>
    /// <param name="dateFrom"></param>
    /// <param name="dateTo"></param>
    /// <param name="category">a negative value indicates to return all categories except tat one e.g Awareness Dates</param>
    /// <param name="ministry">Can be a Guid or an abbrevation (to return all ministries except that one e.g CITENG)</param>
    /// <param name="communicationContactId"></param>
    /// <param name="governmentRepresentativeId"></param>
    /// <param name="premierRequestedId"></param>
    /// <param name="distributionId"></param>
    /// <param name="initiativeId"></param>
    /// <param name="keywordIds"></param>
    /// <param name="dateConfirmed"></param>
    /// <param name="isIssue"></param>
    /// <param name="isThisDayOnly"></param>
    /// <param name="quickSearch"></param>
    /// <param name="includeReviewMarkup"></param>
    /// <param name="excludeshared"></param>
    /// <param name="keyActivities"></param>
    /// <param name="myFavorites"></param>
    /// <returns></returns>
    public static List<ActiveActivity> GetActivitiesSecurely(int systemUserId,
        IList<Guid> nonOwnerMinistryIds,
        int? statusId,
        DateTime dateFrom,
        DateTime? dateTo,
        int? category,
        string ministry,
        int? communicationContactId,
        int? governmentRepresentativeId,
        int? premierRequestedId,
        int? distributionId,
        int? initiativeId,
        IList<string> keywordIds,
        bool? dateConfirmed,
        bool? isIssue,
        bool? isThisDayOnly,
        string quickSearch,
        bool includeReviewMarkup,
        bool? excludeshared,
        bool? keyActivities,
        bool? myFavorites)
    {
        try
        {
            using (var dc = new CorporateCalendarDataContext())
            {
                if (!string.IsNullOrEmpty(quickSearch))
                {
                    int activityId;
                    string[] activityIdString = quickSearch.Split('-');
                    if (int.TryParse(activityIdString[activityIdString.Length - 1], out activityId) && activityId > 10000)
                    {
                        return (from a in dc.ActiveActivities
                                where a.Id == activityId
                                select a).ToList();
                    }
                }

                List<ActiveActivity> filteredActiveActivities =
                        GetUserAccessibleActiveActivitiesWithFilter(systemUserId,
                                                                    nonOwnerMinistryIds,
                                                                    statusId,
                                                                    dateFrom,
                                                                    dateTo,
                                                                    category,
                                                                    ministry,
                                                                    communicationContactId,
                                                                    governmentRepresentativeId,
                                                                    premierRequestedId,
                                                                    distributionId,
                                                                    initiativeId,
                                                                    keywordIds?.Select(k => Convert.ToInt32(k)),
                                                                    dateConfirmed,
                                                                    isIssue,
                                                                    isThisDayOnly == true,
                                                                    includeReviewMarkup,
                                                                    excludeshared,
                                                                    keyActivities,
                                                                    myFavorites);

                if (!string.IsNullOrEmpty(quickSearch))
                {
                    List<ActiveActivity> filteredActiveActivitiesWithTitleSearchApplied = filteredActiveActivities.FindAll(
                        delegate (ActiveActivity p)
                        {
                            return p.Title.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1
                                || (p.Details != null && p.Details.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.City != null && p.City.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.Translations != null && p.Translations.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.Significance != null && p.Significance.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.Comments != null && p.Comments.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.LeadOrganization != null && p.LeadOrganization.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.Strategy != null && p.Strategy.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.HqComments != null && p.HqComments.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.Schedule != null && p.Schedule.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1)
                                || (p.Venue != null && p.Venue.IndexOf(quickSearch, StringComparison.OrdinalIgnoreCase) != -1);

                        }
                        );

                    return filteredActiveActivitiesWithTitleSearchApplied;
                }

                return filteredActiveActivities;
            }
        }
        catch (Exception ex)
        {
            return new List<ActiveActivity>() { new ActiveActivity { Title = "System Error: " + ex.Message, Details = ministry ?? " ", StartDateTime = dateFrom, EndDateTime = dateFrom, IsConfirmed = true, CreatedDateTime = dateFrom, ContactNumber = systemUserId.ToString() } };
        }

    }

    public static List<ActiveActivity> GetUserAccessibleActiveActivitiesWithFilter(int systemUserId,
                                                                                   IList<Guid> nonOwnerMinistryIds,
                                                                                   int? statusId,
                                                                                   DateTime dateFrom,
                                                                                   DateTime? dateTo,
                                                                                   int? category,
                                                                                   string ministry,
                                                                                   int? communicationContactId,
                                                                                   int? governmentRepresentativeId,
                                                                                   int? premierRequestedId,
                                                                                   int? distributionId,
                                                                                   int? initiativeId,
                                                                                   IEnumerable<int> keywordIds,
                                                                                   bool? dateConfirmed,
                                                                                   bool? isIssue,
                                                                                   bool isThisDayOnly,
                                                                                   bool includeReviewMarkup,
                                                                                   bool? excludeshared,
                                                                                   bool? keyActivities,
                                                                                   bool? myFavorites)
    {
        using (var dc = new CorporateCalendarDataContext())
        {
            Guid? ministryGuid = null;
            if (ministry != null && ministry.Length == 36)
            {
                ministryGuid = Guid.Parse(ministry);
                if (nonOwnerMinistryIds != null && !nonOwnerMinistryIds.Any(e => e == ministryGuid))
                {
                    throw new Exception("Unauthorized Ministry Id");
                }
            }

            if (isThisDayOnly) dateTo = dateFrom;

            DateTime? nextDayForDateTo = dateTo.HasValue ? (DateTime?)dateTo.Value.AddDays(1) : null;

            List<ActiveActivity> allActivities;
            if (dateFrom.Year < 2011)
                dateFrom = new DateTime(2011, 1, 1);

            IQueryable<ActiveActivity> activitiesQuery = dc.ActiveActivities;
            if (statusId == 1)
            {
                //Show deleted for review items if 'Changed' status is selected
                activitiesQuery = activitiesQuery.Where(a => (a.StatusId == 1 && a.IsActive) || a.IsActiveNeedsReview);
            }
            else if (includeReviewMarkup && statusId == null)
            {
                //Only Admin/HQ can see the deleted for review item when no status is specified
                activitiesQuery = activitiesQuery.Where(a => a.IsActiveNeedsReview ? !a.IsActive : a.IsActive);
            }
            else
            {
                activitiesQuery = activitiesQuery.Where(a => a.IsActive);
                if (statusId != null)
                {
                    activitiesQuery = activitiesQuery.Where(a => a.StatusId == statusId);
                }
            }

            if (governmentRepresentativeId != null) activitiesQuery = activitiesQuery.Where(a => a.GovernmentRepresentativeId == governmentRepresentativeId);
            if (premierRequestedId != null) activitiesQuery = activitiesQuery.Where(a => a.PremierRequestedId == premierRequestedId);
            if (distributionId != null) activitiesQuery = activitiesQuery.Where(a => a.NRDistributionId == distributionId);
            if (dateConfirmed != null) activitiesQuery = activitiesQuery.Where(a => dateConfirmed == a.IsConfirmed || (a.StartDateTime.Value.Date == a.EndDateTime.Value.Date && !a.IsAllDay));
            if (isIssue != null) activitiesQuery = activitiesQuery.Where(a => a.IsIssue == isIssue);
            if (initiativeId != null) activitiesQuery = activitiesQuery.Where(a => dc.ActivityInitiatives.Any(e => e.ActivityId == a.Id && e.InitiativeId == initiativeId));
            if (category != null)
            {
                if (category.Value >= 0)
                {
                    activitiesQuery = activitiesQuery.Where(a => dc.ActivityCategories.Any(e => e.ActivityId == a.Id && e.CategoryId == category));
                }
                else // reverse Category logic to return all but that category
                {
                    activitiesQuery = activitiesQuery.Where(a => !dc.ActivityCategories.Any(e => e.ActivityId == a.Id && e.CategoryId == -category));
                }
            }
            if (excludeshared == true)
            {
                //Exclude shared ministries AKA only the user's or searched ministry(ies)
                if (ministryGuid == null)
                {
                    activitiesQuery = activitiesQuery.Where(a => dc.SystemUserMinistries.Any(e => e.MinistryId == a.ContactMinistryId && e.SystemUserId == systemUserId && e.IsActive));
                    if (ministry != null)
                    {
                        activitiesQuery = activitiesQuery.Where(a => a.Ministry != ministry); // all but that ministry (e.g CITENG)
                    }
                }
                else
                {
                    activitiesQuery = activitiesQuery.Where(a => a.ContactMinistryId == ministryGuid);
                }
            }


            if (communicationContactId != null) activitiesQuery = activitiesQuery.Where(a => dc.ActiveCommunicationContacts.Any(e => a.CommunicationContactId == e.Id && e.SystemUserId == communicationContactId));
            if (isThisDayOnly) activitiesQuery = activitiesQuery.Where(a => a.StartDateTime >= dateFrom && a.EndDateTime < nextDayForDateTo);
            else activitiesQuery = activitiesQuery.Where(a => a.EndDateTime >= dateFrom && (!nextDayForDateTo.HasValue || a.StartDateTime < nextDayForDateTo));
            if (myFavorites != null) activitiesQuery = activitiesQuery.Where(a => dc.FavoriteActivities.Any(e => e.ActivityId == a.Id && e.SystemUserId == systemUserId));
            if (keyActivities != null) activitiesQuery = activitiesQuery.Where(a => a.IsMilestone);
            if (keywordIds != null)
            {
                int keywordCount = keywordIds.Count();
                IQueryable<ActivityKeyword> activityKeywords = dc.ActivityKeywords.Where(k => keywordIds.Contains(k.KeywordId));
                // get the activities that contain one of the keywords
                activitiesQuery = activitiesQuery.Where(a => activityKeywords.Any(ak => ak.ActivityId == a.Id));
            }
            allActivities = activitiesQuery.ToList();
            //ActivitySharedWiths = (from a in activitiesQuery
            //                       from s in dc.ActivitySharedWiths
            //                       where a.Id == s.ActivityId
            //                       select s).ToArray();


            if (excludeshared == true)
            {
                return allActivities;
            }
            List<ActiveActivity> activities = new List<ActiveActivity>();
            foreach (var a in allActivities)
            {
                bool include = true;

                string[] sharedMinistriedIDs = a.SharedWithMinistryIds?.ToLower()?.Split(',') ?? new string[] { };
                if (ministryGuid == null)
                {
                    if (nonOwnerMinistryIds != null)
                    {
                        //Show all activities in user (non app owner) ministries or shared
                        include = nonOwnerMinistryIds.Any(e => e == a.ContactMinistryId)
                                   || sharedMinistriedIDs.Any(s => nonOwnerMinistryIds.Any(e => e.ToString() == s));
                    } // else Show all activities
                    if (ministry != null)
                    {
                        include &= a.Ministry != ministry; // all but that ministry (e.g CITENG)
                    }
                }
                else
                {
                    //Ministry ID is not null, so show all activities for that ministry plus activities that are shared with that ministry
                    include = a.ContactMinistryId == ministryGuid; //all activities in the ministry 
                    //or the activity is shared with the selected ministry and is not internal
                    include |= sharedMinistriedIDs.Any(s => s == ministryGuid.ToString());
                }

                if (include)
                {
                    activities.Add(a);
                }
            }
            return activities;
        }
    }

    /// <summary>
    /// Search for future activities or activities' end date time is after today and before specific date time
    /// </summary>
    /// <param name="systemUserId"></param>
    /// <param name="isCurrentUserInOwnerList"></param>
    /// <param name="endDateTime"></param>
    /// <param name="statuses"></param>
    /// <param name="hqStatuses"></param>
    /// <param name="includeDeleted"></param>
    /// <returns></returns>
    public static List<ActiveActivity> GetAllActivitiesSecurely(int systemUserId,
                                                                bool isCurrentUserInOwnerList,
                                                                DateTime? endDateTime,
                                                                string[] statuses,
                                                                string[] hqStatuses,
                                                                bool includeDeleted)
    {
        bool includeChanged = statuses.Contains("Changed");

        using (var dc = new CorporateCalendarDataContext())
        {
            IQueryable<ActiveActivity> activities;

            if (endDateTime < DateTime.Today)
            {

                var daysBetweenTodayAndEndDateTime = (endDateTime - DateTime.Today).Value.Days;

                activities = from a in dc.ActiveActivities
                             let ministries = dc.SystemUserMinistries.Where(m => m.SystemUserId == systemUserId && m.IsActive).Select(m => m.MinistryId)
                             let sharedMinistries = dc.ActivitySharedWiths.Where(asw => asw.ActivityId == a.Id).Select(asw => asw.MinistryId)
                             where (ministries.Any(i => i == a.ContactMinistryId) || ministries.Where(f => sharedMinistries.Any(s => s == f)).Any()
                                        || isCurrentUserInOwnerList)
                                    && (a.CreatedDateTime >= DateTime.Now.AddDays(daysBetweenTodayAndEndDateTime))
                                    //Changed/New/Reviewed activities
                                    && ((hqStatuses.Contains(a.HqStatus)) ||
                                         (statuses.Contains(a.Status) && a.IsActive) ||
                                         //Included the deleted and to be reviewed activities if Changed status is requested
                                         (includeChanged && !a.IsActive && a.IsActiveNeedsReview) ||
                                         //Deleted activities
                                         (includeDeleted && !a.IsActive))
                             select a;

                return activities.ToList();
            }

            activities = from a in dc.ActiveActivities
                         let ministries = dc.SystemUserMinistries.Where(m => m.SystemUserId == systemUserId && m.IsActive).Select(m => m.MinistryId)
                         let sharedMinistries = dc.ActivitySharedWiths.Where(asw => asw.ActivityId == a.Id).Select(asw => asw.MinistryId)
                         where (ministries.Any(i => i == a.ContactMinistryId) || ministries.Where(f => sharedMinistries.Any(s => s == f)).Any()
                                    || isCurrentUserInOwnerList)
                                && (endDateTime == null || a.StartDateTime <= endDateTime) && a.EndDateTime > DateTime.Today
                                //Changed/New/Reviewed activities
                                && ((hqStatuses.Contains(a.HqStatus)) ||
                                     (statuses.Contains(a.Status) && a.IsActive) ||
                                     //Included the deleted and to be reviewed activities if Changed status is requested
                                     (includeChanged && !a.IsActive && a.IsActiveNeedsReview) ||
                                     //Deleted activities
                                     (includeDeleted && !a.IsActive))
                         select a;

            return activities.ToList();
        }
    }

}