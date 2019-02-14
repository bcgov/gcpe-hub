using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using CorporateCalendar.Data;
using CorporateCalendar.Security;

/// <summary>
/// Summary description for ActivityManager
/// </summary>
public class ActivityManager
{

    public ActivityManager()
    {
        //
        // TODO: Add constructor logic here
        //
    }

    public static bool ReviewActivity(int activityId, string lastUpdatedDateTime) 
    {
        var dc = new CorporateCalendar.Data.CorporateCalendarDataContext(); //"param1", "param2", "param3");
        var activities = from a in dc.Activities
                         where a.Id == activityId
                         select a;

        if (activities.Count() > 0)
        {
            var activity = activities.First();

            if (activity.IsActive == false)
            {
                activity.IsActiveNeedsReview = false;
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(lastUpdatedDateTime) 
                    && lastUpdatedDateTime != (activity.LastUpdatedDateTime ?? activity.CreatedDateTime ?? DateTime.MinValue).ToOADate().ToString())
                {
                     return false;
                }
                //TODO: 2 should be a constant (Trinity's note when I came across this)
                //TODO: I noticed in ActivityHandler.ashx they also set activity.IsReviewed = true, does this need to be done too?
                activity.StatusId = 2; // status id is 2 means a Reviewed
                //Need to also mark the columns that flag columns
                activity.IsTitleNeedsReview = false;
                activity.IsDetailsNeedsReview = false;
                activity.IsCityNeedsReview = false;
                activity.IsRepresentativeNeedsReview = false;
                activity.IsStartDateNeedsReview = false;
                activity.IsEndDateNeedsReview = false;
                activity.IsCategoriesNeedsReview = false;
                activity.IsCommMaterialsNeedsReview = false;
            }
            //activity.IsReviewed = true;
            dc.SubmitChanges();
            
        }
        return true;
    }

    public static void DeleteActivity(int activityId)
    {
        var dc = new CorporateCalendar.Data.CorporateCalendarDataContext(); //"param1", "param2", "param3");
        var activities = from a in dc.Activities
                         where a.Id == activityId
                         select a;

        if (activities.Count() > 0)
        {
            var activity = activities.First();
            activity.IsActive = false; // Set IsActive = false, means deleted
            activity.IsActiveNeedsReview = true;
            //activity.IsReviewed = false;
            dc.SubmitChanges();
        }
    }

    /// <summary>
    /// This is used from the Activity page
    /// </summary>
    public static void UpdateActivity(CorporateCalendarDataContext db, CustomPrincipal customPrincipal, Activity activity,
        string title, string details, DateTime? startDateTime, DateTime? endDateTime, string potentialDates, int? representativeId, int? cityId, string otherCity,
        bool categoryHasChanged, bool commMaterialsHaveChanged, bool isIssue, bool isConfidential, string translationsRequired, Dictionary<string, string> dropDownListValues)
    {
        //-- Need to check if these fields were modified and mark them
        //--Title and Details
        if (activity.Title != title)
        {
            activity.Title = title;
            activity.IsTitleNeedsReview = true;
            activity.StatusId = 1; // Changed
        }

        if (activity.Details != details)
        {
            activity.Details = details;
            activity.IsDetailsNeedsReview = true;
            activity.StatusId = 1; // Changed
        }

        if (activity.GovernmentRepresentativeId != representativeId)
        {
            activity.GovernmentRepresentativeId = representativeId;
            activity.IsRepresentativeNeedsReview = true;
            activity.StatusId = 1; // Changed
        }

        if (activity.CityId != cityId || otherCity != activity.OtherCity)
        {
            activity.CityId = cityId;
            activity.OtherCity = otherCity;
            activity.IsCityNeedsReview = true;
            activity.StatusId = 1; // Changed
        }

        if (activity.StartDateTime != startDateTime)
        {
            activity.StartDateTime = startDateTime;
            activity.IsStartDateNeedsReview = true;
            activity.StatusId = 1; // Changed
        }

        if (activity.EndDateTime != endDateTime)
        {
            activity.EndDateTime = endDateTime;
            activity.IsEndDateNeedsReview = true;
            activity.StatusId = 1; // Changed
        }


        if ((activity.PotentialDates??"") != potentialDates)
        {
            activity.PotentialDates = potentialDates;
            activity.IsStartDateNeedsReview = true;
            activity.StatusId = 1; // Changed
        }

        if (categoryHasChanged || activity.IsIssue != isIssue || activity.IsConfidential != isConfidential)
        {
            activity.IsIssue = isIssue;
            activity.IsConfidential = isConfidential;
            activity.IsCategoriesNeedsReview = true;
            activity.StatusId = 1; // Changed
        }

        if (activity.Translations != translationsRequired)
        {
            activity.Translations = translationsRequired;
            activity.StatusId = 1; // Changed
        }

        if (commMaterialsHaveChanged)
        {
            activity.IsCommMaterialsNeedsReview = true;
            activity.StatusId = 1; // Changed
        }

        #region Log History
        using (var transactionScope = new TransactionScope())
        {
            //bool hasBeenConfirmed = false;
            //bool hasBeenUnConfirmed = false;

            foreach (System.Data.Linq.ModifiedMemberInfo mmi in db.Activities.GetModifiedMembers(activity))
            {
                string memberName = mmi.Member.Name;
                string fromValue = string.Empty;
                string toValue = null;
                if (mmi.CurrentValue != null)
                {
                    toValue = mmi.CurrentValue.ToString(); // There's always a current value! (Except when there isn't)
                }
                if (mmi.OriginalValue != null)
                {
                    fromValue = mmi.OriginalValue.ToString();
                    // Handle IsConfirmed
                    /*if (memberName == "IsConfirmed")
                    {
                        // If this activity was originally confirmed and is currently unconfirmed, set the flag
                        if ((bool)mmi.OriginalValue == true && (bool)mmi.CurrentValue == false)
                        {
                            // This activity has been UNconfirmed.
                            hasBeenUnConfirmed = true;
                        }
                        else if ((bool)mmi.CurrentValue == true)
                        {
                            // All other cases are confirmations from previously unconfirmed states
                            hasBeenConfirmed = true;
                        }
                    }*/
                }

                switch (memberName)
                {
                    case "Title":
                    case "StartDateTime":
                    case "EndDateTime":
                    case "CityId":
                    case "IsConfirmed":
                    case "CommunicationContactId":
                    case "ContactMinistryId":
                    case "ActivityCategoriesId":
                    case "ActivityCommunicationMaterialsId":
                    case "NewsReleaseOriginId":
                    case "NewsReleaseDistributionId":
                    case "PriorityId":
                    case "ActivityThemesId":
                        var newchange = new CorporateCalendar.Data.Log();
                        newchange.ActivityId = activity.Id;
                        newchange.LogType = Convert.ToInt32(CorporateCalendar.Logging.Log.LogType.Message);
                        newchange.OldValue = fromValue;
                        newchange.NewValue = toValue;
                        newchange.Operation = CorporateCalendar.Logging.Log.OperationType.Update.ToString();
                        newchange.CreatedBy = customPrincipal.Id;
                        newchange.CreatedDateTime = DateTime.Now;
                        newchange.TableName = "Activity";
                        newchange.LastUpdatedBy = newchange.CreatedBy;
                        newchange.LastUpdatedDateTime = DateTime.Now;
                        newchange.IsActive = true;
                        newchange.FieldName = memberName; // you may want to change to more human readable?
                        db.Logs.InsertOnSubmit(newchange);
                        break;
                    default:
                        break;
                }
            }


            ActivityWebService.UpdateLinkingTables(activity.Id, dropDownListValues, customPrincipal, db);

            db.SubmitChanges();
            transactionScope.Complete();
        }
        #endregion Log History
    }

}