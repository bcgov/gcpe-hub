using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Services;
using System.Transactions;
using CorporateCalendar.Data;

/// <summary>
/// Summary description for ActivityWebService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
[System.Web.Script.Services.ScriptService]
public class ActivityWebService : System.Web.Services.WebService
{

    private static CorporateCalendarDataContext CorporateCalendarDataContext
    {
        get
        {
            var corporateCalendarDataContext = new CorporateCalendarDataContext(); //"param1", "param2", "param3");
            return corporateCalendarDataContext;
        }
    }



    public ActivityWebService()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    //[WebMethod]
    //public string HelloWorld() {
    //    System.Threading.Thread.Sleep(1500);
    //    return "Hello World";
    //}

    [WebMethod]
    public static bool SubmitSuggestion()
    {
        string suggestion = "safdsafdsafas";
        var log = new CorporateCalendar.Logging.Log(suggestion, CorporateCalendar.Logging.Log.LogType.Suggestion, true);
        return true;
    }

    [WebMethod]
    public static ActiveActivity GetActiveActivityById(int activityId)
    {
        IQueryable<ActiveActivity> activity = CorporateCalendarDataContext.ActiveActivities.Where(a => a.Id == activityId);
        return activity.FirstOrDefault();
    }

    [WebMethod]
    public static List<ActiveActivity> GetAllActiveActivities()
    {
        throw new NotImplementedException();
    }


    [WebMethod]
    public static List<ActiveActivity> GetAllActiveActivitiesForSystemUser(int systemUserId)
    {
        throw new NotImplementedException();
    }


    [WebMethod]
    public static int? CloneActiveActivity(ActiveActivity activity, Dictionary<string, string> dropDownListValues, CorporateCalendar.Security.CustomPrincipal customPrincipal)
    {
        var db = CorporateCalendarDataContext; // We need to maintain a common context

        dropDownListValues.Remove("ActivityTagIds"); // remove news subscriptions when cloning

        #region Clone the activity
        using (var transactionScope = new TransactionScope())
        {
            var clone = new CorporateCalendar.Data.Activity
            {
                Title = activity.Title,
                StartDateTime = activity.StartDateTime,
                EndDateTime = activity.EndDateTime,
                NRDateTime = null,
                Comments = string.Empty,
                IsInternalNotesNeedsReview = false,
                Details = activity.Details,
                IsAllDay = activity.IsAllDay,
                CreatedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.Now,
                CreatedBy = customPrincipal.Id,
                LastUpdatedBy = customPrincipal.Id,
                ContactMinistryId = activity.ContactMinistryId,
                // Single-select
                CommunicationContactId = activity.CommunicationContactId,
                // Single-select
                GovernmentRepresentativeId = activity.GovernmentRepresentativeId,
                // Single-select
                CityId = activity.CityId,
                // Single-select
                NRDistributionId = activity.NRDistributionId,
                // Single-select
                PremierRequestedId = activity.PremierRequestedId,
                // Single-select
                Venue = activity.Venue,

                IsConfirmed = activity.IsConfirmed,
                PotentialDates = activity.PotentialDates,
                IsConfidential = activity.IsConfidential,
                IsCrossGovernment = activity.IsCrossGovernment,
                IsAtLegislature = activity.IsAtLegislature,
                HqSection = activity.HqSection, // not HqComments
                HqComments = "**",
                StatusId = 7, // New
                IsActive = true,
                LeadOrganization = activity.LeadOrganization,
                Significance = activity.Significance,
                Strategy = activity.Strategy,
                Schedule = activity.Schedule,
                OtherCity = activity.OtherCity,
                IsIssue = activity.IsIssue,
                IsMilestone = activity.IsMilestone,
            };

            db.Activities.InsertOnSubmit(clone);
            db.SubmitChanges();

            int cloneId = clone.Id;

            UpdateLinkingTables(cloneId, dropDownListValues, customPrincipal, db, cloningActivity: true);
            db.SubmitChanges();
            transactionScope.Complete();

            return cloneId;
            #endregion
        }
    }

    [WebMethod]
    public static bool ReviewActiveActivity(int activityId)
    {
        throw new NotImplementedException();
    }

    [WebMethod]
    public static bool ReviewMultipleActiveActivities(List<int> activityIdList)
    {
        // Use this method for both ReviewSelectedActiveActivities and ReviewAllActiveActivities (in grid)
        throw new NotImplementedException();
    }

    [WebMethod]
    public static bool DeleteActiveActivity(int activityId)
    {
        throw new NotImplementedException();
    }

    [WebMethod]
    public static bool DeleteMultipleActiveActivities(List<int> activityIdList)
    {
        // Use this method for both DeleteSelectedActiveActivities and DeleteAllActiveActivities (in grid)
        throw new NotImplementedException();
    }

    [WebMethod]
    public static int? InsertActiveActivity(CorporateCalendar.Data.Activity activity, Dictionary<string, string> dropDownListValues, CorporateCalendar.Security.CustomPrincipal customPrincipal)
    {
        var db = CorporateCalendarDataContext;

        using (var transactionScope = new TransactionScope())
        {

            var newActivity = new CorporateCalendar.Data.Activity
            {
                Title = activity.Title,
                StartDateTime = activity.StartDateTime,
                EndDateTime = activity.EndDateTime,
                Comments = activity.Comments,
                Details = activity.Details,
                IsAllDay = activity.IsAllDay,
                CreatedDateTime = DateTime.Now,
                LastUpdatedDateTime = DateTime.Now,
                CreatedBy = customPrincipal.Id,
                LastUpdatedBy = customPrincipal.Id,
                ContactMinistryId = activity.ContactMinistryId,
                // Single-select
                CommunicationContactId = activity.CommunicationContactId,
                // Single-select
                GovernmentRepresentativeId = activity.GovernmentRepresentativeId,
                // Single-select
                CityId = activity.CityId,
                // Single-select
                NRDistributionId = activity.NRDistributionId,
                // Single-select
                PremierRequestedId = activity.PremierRequestedId,
                // Single-select
                Venue = activity.Venue,
                IsConfirmed = activity.IsConfirmed,
                IsConfidential = activity.IsConfidential,
                IsCrossGovernment = activity.IsCrossGovernment,
                IsAtLegislature = activity.IsAtLegislature,
                HqSection = activity.HqSection,
                HqStatusId = activity.HqStatusId,
                StatusId = 7, // New
                IsActive = true
            };

            db.Activities.InsertOnSubmit(newActivity);
            db.SubmitChanges();

            int newActivityId = newActivity.Id;

            UpdateLinkingTables(newActivityId, dropDownListValues, customPrincipal, db);

            db.SubmitChanges();
            transactionScope.Complete();

            return newActivityId;
        }

    }


    public static void UpdateLinkingTables(int activityId, Dictionary<string, string> dropDownListValues,
        CorporateCalendar.Security.CustomPrincipal customPrincipal, CorporateCalendarDataContext db, bool cloningActivity = false)
    {
        foreach (var dropDownList in dropDownListValues)
        {
            char separator = dropDownList.Key == "ActivityKeywords" ? '~' : ',';
            string[] dropDownValues = string.IsNullOrEmpty(dropDownList.Value) ? new string[0] : dropDownList.Value.Split(separator).Where(e => !string.IsNullOrWhiteSpace(e)).ToArray();
            switch (dropDownList.Key)
            {
                case "ActivitySharedWithIds":
                    // Even if there were no shared ministries selected, delete those that may have been selected before
                    var currentSharedWith =
                        db.ActivitySharedWiths.Where(p => p.ActivityId == activityId);
                    if (currentSharedWith.Any())
                        db.ActivitySharedWiths.DeleteAllOnSubmit(currentSharedWith);

                    foreach (
                        var activitySharedWith in
                            dropDownValues.Select(ministryId => new ActivitySharedWith
                            {
                                ActivityId = activityId,
                                MinistryId = Guid.Parse(ministryId),
                                IsActive = true,
                                LastUpdatedBy = customPrincipal.Id,
                                CreatedBy = customPrincipal.Id,
                                CreatedDateTime = DateTime.Now,
                                LastUpdatedDateTime = DateTime.Now
                            }))
                    {
                        db.ActivitySharedWiths.InsertOnSubmit(activitySharedWith);
                        // Submit changes only after
                    }
                    break;
                case "ActivityCategoryIds":
                    var currentCategories =
                        db.ActivityCategories.Where(p => p.ActivityId == activityId);
                    if (currentCategories.Any())
                        db.ActivityCategories.DeleteAllOnSubmit(currentCategories);

                    foreach (
                        var activityCategory in dropDownValues.Select(categoryId => new ActivityCategory
                        {
                            ActivityId = activityId,
                            CategoryId = Convert.ToInt32(categoryId),
                            IsActive = true,
                            LastUpdatedBy = customPrincipal.Id,
                            CreatedBy = customPrincipal.Id,
                            CreatedDateTime = DateTime.Now,
                            LastUpdatedDateTime = DateTime.Now
                        }))
                    {
                        db.ActivityCategories.InsertOnSubmit(activityCategory);
                        // Submit changes only after
                    }
                    break;
                case "ActivityCommunicationMaterialIds":
                    // Always delete old items
                    var currentCommunicationMaterials =
                        db.ActivityCommunicationMaterials.Where(p => p.ActivityId == activityId);
                    if (currentCommunicationMaterials.Any())
                        db.ActivityCommunicationMaterials.DeleteAllOnSubmit(currentCommunicationMaterials);

                    foreach (
                        var activityCommunicationMaterial in
                            dropDownValues.Select(
                                communicationMaterialId => new ActivityCommunicationMaterial
                                {
                                    ActivityId = activityId,
                                    CommunicationMaterialId =
                                        Convert.ToInt32(communicationMaterialId),
                                    IsActive = true,
                                    LastUpdatedBy = customPrincipal.Id,
                                    CreatedBy = customPrincipal.Id,
                                    CreatedDateTime = DateTime.Now,
                                    LastUpdatedDateTime = DateTime.Now
                                }))
                    {
                        db.ActivityCommunicationMaterials.InsertOnSubmit(activityCommunicationMaterial);
                        // Submit changes only after
                    }
                    break;
                case "ActivityNewsReleaseOriginIds":
                    // Always delete old items
                    var currentNewsReleaseOrigins =
                        db.ActivityNROrigins.Where(p => p.ActivityId == activityId);
                    if (currentNewsReleaseOrigins.Any())
                        db.ActivityNROrigins.DeleteAllOnSubmit(currentNewsReleaseOrigins);

                    foreach (
                        var activityNewsReleaseOrigin in
                            dropDownValues.Select(
                                newsReleaseOriginId => new ActivityNROrigin
                                {
                                    ActivityId = activityId,
                                    NROriginId = Convert.ToInt32(newsReleaseOriginId),
                                    IsActive = true,
                                    LastUpdatedBy = customPrincipal.Id,
                                    CreatedBy = customPrincipal.Id,
                                    CreatedDateTime = DateTime.Now,
                                    LastUpdatedDateTime = DateTime.Now
                                }))
                    {
                        db.ActivityNROrigins.InsertOnSubmit(activityNewsReleaseOrigin);
                        // Submit changes only after
                    }
                    break;

                case "ActivitySectorIds":
                    var currentSectors =
                        db.ActivitySectors.Where(p => p.ActivityId == activityId);
                    if (currentSectors.Any())
                        db.ActivitySectors.DeleteAllOnSubmit(currentSectors);

                    foreach (var activitySector in dropDownValues.Select(sectorId => new ActivitySector()
                    {
                        ActivityId = activityId,
                        SectorId = Guid.Parse(sectorId),
                        IsActive = true,
                        LastUpdatedBy = customPrincipal.Id,
                        CreatedBy = customPrincipal.Id,
                        CreatedDateTime = DateTime.Now,
                        LastUpdatedDateTime = DateTime.Now
                    }))
                    {
                        db.ActivitySectors.InsertOnSubmit(activitySector);
                        // Submit changes only after
                    }
                    break;
                case "ActivityThemeIds":
                    // Always delete old items
                    var currentThemes =
                        db.ActivityThemes.Where(p => p.ActivityId == activityId);
                    if (currentThemes.Any())
                        db.ActivityThemes.DeleteAllOnSubmit(currentThemes);
                    foreach (
                        var activityTheme in dropDownValues.Select(themeId => new ActivityTheme()
                        {
                            ActivityId = activityId,
                            ThemeId = Guid.Parse(themeId),
                            IsActive = true,
                            LastUpdatedBy = customPrincipal.Id,
                            CreatedBy = customPrincipal.Id,
                            CreatedDateTime = DateTime.Now,
                            LastUpdatedDateTime = DateTime.Now
                        }))
                    {
                        db.ActivityThemes.InsertOnSubmit(activityTheme);
                        // Submit changes only after
                    }
                    break;
                case "ActivityTagIds":
                    // Always delete old items
                    var currentTags =
                        db.ActivityTags.Where(p => p.ActivityId == activityId);
                    if (currentTags.Any())
                        db.ActivityTags.DeleteAllOnSubmit(currentTags);
                    foreach (
                        var activityTag in dropDownValues.Select(tagId => new ActivityTag()
                        {
                            ActivityId = activityId,
                            TagId = Guid.Parse(tagId),
                            IsActive = true,
                            LastUpdatedBy = customPrincipal.Id,
                            CreatedBy = customPrincipal.Id,
                            CreatedDateTime = DateTime.Now,
                            LastUpdatedDateTime = DateTime.Now
                        }))
                    {
                        db.ActivityTags.InsertOnSubmit(activityTag);
                        // Submit changes only after
                    }
                    break;
                case "ActivityKeywords":
                    var previousKeywords =
                        db.ActivityKeywords.Where(p => p.ActivityId == activityId);

                    var newKeywordsNames = dropDownValues.Select(e => e.Trim()).Distinct().ToList();
                    var newKeywords = db.Keywords.Where(k => newKeywordsNames.Contains(k.Name)).ToList();

                    // update the date/time of previous keywords and delete old ones
                    foreach (var previousKeyword in previousKeywords)
                    {
                        var newKeyword = newKeywords.FirstOrDefault(k => k.Id == previousKeyword.KeywordId);
                        if (newKeyword != null)
                        {
                            newKeyword.LastUpdatedDateTime = DateTime.Now;
                            newKeywordsNames.Remove(newKeyword.Name);
                        }
                        else
                        {
                            if (db.ActivityKeywords.Count(k => k.KeywordId == previousKeyword.KeywordId) == 1)
                            {
                                // cleanup keywords not used anymore
                                db.Keywords.DeleteOnSubmit(db.Keywords.First(k => k.Id == previousKeyword.KeywordId));
                            }
                            db.ActivityKeywords.DeleteOnSubmit(previousKeyword);
                        }
                    }

                    // add new keyword associations. Create a brand new keyword if none exist yet.
                    foreach (var keywordName in newKeywordsNames)
                    {
                        var newKeyword = newKeywords.FirstOrDefault(k => k.Name == keywordName);

                        if (cloningActivity == true && newKeyword.Name != "30-60-90") continue;

                        if (newKeyword == null)
                        {
                            newKeyword = new Keyword()
                            {
                                Name = keywordName,
                                IsActive = true,
                                LastUpdatedBy = customPrincipal.Id,
                                LastUpdatedDateTime = DateTime.Now
                            };
                            db.Keywords.InsertOnSubmit(newKeyword);
                            db.SubmitChanges();
                        }
                        else
                        {
                            newKeyword.LastUpdatedDateTime = DateTime.Now;
                        }

                        var activityKeyword = new ActivityKeyword()
                        {
                            ActivityId = activityId,
                            KeywordId = newKeyword.Id,
                            IsActive = true,
                            LastUpdatedBy = customPrincipal.Id,
                            LastUpdatedDateTime = DateTime.Now
                        };
                        db.ActivityKeywords.InsertOnSubmit(activityKeyword);
                    }
                    // Submit changes only after
                    break;
                case "ActivityInitiativeIds":
                    // Always delete old items
                    var currentInitiatives =
                        db.ActivityInitiatives.Where(p => p.ActivityId == activityId);
                    if (currentInitiatives.Any())
                        db.ActivityInitiatives.DeleteAllOnSubmit(currentInitiatives);

                    foreach (
                        var ActivityInitiative in dropDownValues.Select(initiativeId => new ActivityInitiative()
                        {
                            ActivityId = activityId,
                            InitiativeId = int.Parse(initiativeId),
                            IsActive = true,
                            LastUpdatedBy = customPrincipal.Id,
                            CreatedBy = customPrincipal.Id,
                            CreatedDateTime = DateTime.Now,
                            LastUpdatedDateTime = DateTime.Now
                        }))
                    {
                        db.ActivityInitiatives.InsertOnSubmit(ActivityInitiative);
                        // Submit changes only after
                    }
                    break;
            }
        }
    }


    [WebMethod]
    public static bool UpdateActiveActivity(CorporateCalendar.Data.Activity activity, Dictionary<string, string> dropDownListValues, CorporateCalendar.Security.CustomPrincipal customPrincipal)
    {
        var db = CorporateCalendarDataContext;

        using (var transactionScope = new TransactionScope())
        {

            // The activity needs to be attached to the current data context
            db.Activities.Attach(activity);

            activity.StatusId = 1;
            UpdateLinkingTables(activity.Id, dropDownListValues, customPrincipal, db);

            db.SubmitChanges();
            transactionScope.Complete();
        }

        return true; // if successful
    }


    [WebMethod]
    public static int? SaveActivity(CorporateCalendar.Data.Activity activity, Dictionary<string, string> dropDownListValues, CorporateCalendar.Security.CustomPrincipal customPrincipal, SaveType saveType)
    {
        using (var transactionScope = new TransactionScope())
        {
            var db = CorporateCalendarDataContext;
            int newActivityId = 0;

            if (saveType == SaveType.Insert)
            {


                var newActivity = new CorporateCalendar.Data.Activity
                {
                    Title = activity.Title,
                    StartDateTime = activity.StartDateTime,
                    EndDateTime = activity.EndDateTime,
                    Comments = activity.Comments,
                    Details = activity.Details,
                    IsAllDay = activity.IsAllDay,
                    CreatedDateTime = DateTime.Now,
                    CreatedBy = customPrincipal.Id,
                    ContactMinistryId = activity.ContactMinistryId,
                    // Single-select
                    CommunicationContactId = activity.CommunicationContactId,
                    // Single-select
                    GovernmentRepresentativeId = activity.GovernmentRepresentativeId,
                    // Single-select
                    CityId = activity.CityId,
                    Venue = activity.Venue,

                    // Single-select
                    //NRDistributionId = activity.NRDistributionId,
                    // Single-select
                    PremierRequestedId = activity.PremierRequestedId,
                    // Single-select

                    IsConfirmed = activity.IsConfirmed,
                    IsConfidential = activity.IsConfidential,
                    IsCrossGovernment = activity.IsCrossGovernment,
                    IsAtLegislature = activity.IsAtLegislature,
                    HqStatusId = activity.HqStatusId,
                    HqSection = activity.HqSection,
                    IsActive = true,

                    // multiselect

                    // Meta Data (The rest of the fields are handled by the database using simple default values)
                    LastUpdatedBy = customPrincipal.Id,
                    LastUpdatedDateTime = DateTime.Now
                };

                newActivity.StatusId = 7;
                db.Activities.InsertOnSubmit(newActivity);
                //db.Log = new ActionTextWriter(s => System.Diagnostics.Debug.Write(s));
                db.SubmitChanges();
                newActivityId = newActivity.Id;
            }
            else
            {

                var newActivity = CorporateCalendarDataContext.Activities.Single(p => p.Id == activity.Id);

                newActivity.Title = activity.Title;
                newActivity.StartDateTime = activity.StartDateTime;
                newActivity.EndDateTime = activity.EndDateTime;
                newActivity.Comments = activity.Comments;
                newActivity.Details = activity.Details;
                newActivity.IsAllDay = activity.IsAllDay;
                newActivity.CreatedDateTime = DateTime.Now;
                newActivity.CreatedBy = customPrincipal.Id;
                newActivity.ContactMinistryId = activity.ContactMinistryId;
                // Single-select
                newActivity.CommunicationContactId = activity.CommunicationContactId;
                // Single-select
                newActivity.GovernmentRepresentativeId = activity.GovernmentRepresentativeId;
                // Single-select
                newActivity.CityId = activity.CityId;
                newActivity.Venue = activity.Venue;

                // Single-select
                newActivity.NRDistributionId = activity.NRDistributionId;
                // Single-select
                newActivity.PremierRequestedId = activity.PremierRequestedId;
                // Single-select

                newActivity.IsConfirmed = activity.IsConfirmed;
                newActivity.IsConfidential = activity.IsConfidential;
                newActivity.IsCrossGovernment = activity.IsCrossGovernment;
                newActivity.IsAtLegislature = activity.IsAtLegislature;
                newActivity.HqSection = activity.HqSection;
                newActivity.HqStatusId = activity.HqStatusId;
                newActivity.IsActive = true;

                // Meta Data (The rest of the fields are handled by the database using simple default values)
                newActivity.LastUpdatedBy = customPrincipal.Id;
                newActivity.LastUpdatedDateTime = DateTime.Now;

                newActivity.StatusId = 1; // Changed


                db.SubmitChanges();
                newActivityId = newActivity.Id; // this is really the original id
            }

            UpdateLinkingTables(newActivityId, dropDownListValues, customPrincipal, db);

            db.SubmitChanges();
            transactionScope.Complete();

            return newActivityId;
        }
    }
}
