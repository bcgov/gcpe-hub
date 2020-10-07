using System;
using System.Linq;
using System.Collections.Generic;

using CorporateCalendar.Data;
using CorporateCalendar.Security;

/// <summary>
/// Summary description for DropDownManager
/// </summary>
public class DropDownListManager
{
    #region Constructors
    CorporateCalendarDataContext corporateCalendarDataContext;
    static List<Guid> noGuids = new List<Guid>();
    static List<int> noIds = new List<int>();
    public DropDownListManager(CorporateCalendarDataContext dc)
    {
        corporateCalendarDataContext = dc;
    }
    #endregion



    #region Status
    /// <summary>
    /// Returns active rows from the Status table.
    /// </summary>
    /// <returns>System.Object (IQueryable<![CDATA[<ActiveGovernmentRepresentative>]]> )</returns>
    public object GetStatusOptions()
    {
        IQueryable<Status> options = corporateCalendarDataContext.Status;
        return options;
    }
    public object GetHQStatusOptions()
    {
        IQueryable<Status> options = corporateCalendarDataContext.Status.Where(p => p.Id != 2); // not Reviewed (Just HQ New or HQ Changed)

        return options;
    }
    #endregion

    #region Government Representatives
    /// <summary>
    /// Returns active rows from the GovernmentRepresentatives table. 
    /// </summary>
    /// <returns>System.Object (IQueryable<![CDATA[<ActiveGovernmentRepresentative>]]> )</returns>
    public object GetActiveGovernmentRepresentativeOptions(int? includedId = null, bool showLegacy = false)
    {
        IQueryable<GovernmentRepresentative> options = corporateCalendarDataContext.GovernmentRepresentatives
        .Where(p => p.IsActive == true || p.Id == includedId);

        if (!showLegacy)
        {
            string[] legacyItems = {
            "N/A",
            "TBC",
            "Premier"
        };

            options = options.Where(e => !legacyItems.Contains(e.Name));
        }

        options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);

        return options;
    }
    #endregion

    #region Shared-With (other Ministries)
    // Get: See Ministry (The shared with dropdownlist is populated with ministry shortnames)
    #endregion

    #region Communication Contacts 

    // View: ActiveCommunicationContacts = All Users + Ministry, SO YOU MAY HAVE DUPIATES
    //  John Smith EDUC
    //  John Smith FIN
    //  John Smith CTIZ

    // View: ActiveDistinctCommunicationContacts = All User, unique
    // John Smith

    public enum CommunicationSortOrder
    {
        FirstName,
        RoleThenName
    }


    /// <summary>
    /// ...
    /// </summary>
    /// <param name="ministryCode"></param>
    /// <param name="sort"></param> 
    /// <returns>Active Communication Contacts for the ministry selected</returns>
    public IQueryable<ActiveDistinctCommunicationContact> GetCommunicationContactByMinistryShortName(string ministryCode, CommunicationSortOrder sort)
    {
        // restrict displayed contacts based on the ministry parameter
        var commcontactsandministry = corporateCalendarDataContext.ActiveCommunicationContacts
            .Where(p => p.MinistryShortName == ministryCode);

        // Get list of the distinct users.
        var distinctcomm = corporateCalendarDataContext.ActiveDistinctCommunicationContacts;

        IQueryable<ActiveDistinctCommunicationContact> options = (from orig in distinctcomm
                                                                  from b in commcontactsandministry
                                                                  where orig.SystemUserId == b.SystemUserId
                                                                  select orig).Distinct();

        if (sort == CommunicationSortOrder.RoleThenName)
        {
            options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);
        }
        else
        {
            options = options.OrderBy(p => p.Name);
        }

        return options;
    }



    /// <summary>
    /// ...
    /// </summary>
    /// <param name="ministryId"></param>
    /// <param name="sort"></param>
    /// <returns>Active Communication Contacts for the ministry selected</returns>
    public IQueryable<ActiveDistinctCommunicationContact> GetCommunicationContactByMinistryId(Guid ministryId, CommunicationSortOrder sort)
    {
        // restrict displayed contacts based on the ministry parameter
        var commcontactsandministry = corporateCalendarDataContext.ActiveCommunicationContacts
            .Where(p => p.MinistryId == ministryId);

        // Get list of the distinct users.
        var distinctcomm = corporateCalendarDataContext.ActiveDistinctCommunicationContacts;

        IQueryable<ActiveDistinctCommunicationContact> options = (from orig in distinctcomm
                                                                  from b in commcontactsandministry
                                                                  where orig.SystemUserId == b.SystemUserId
                                                                  select orig).Distinct();

        if (sort == CommunicationSortOrder.RoleThenName)
        {
            options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);
        }
        else
        {
            options = options.OrderBy(p => p.Name);
        }

        return options;
    }


    /// <summary>
    /// Returns active rows from the CommunicationContacts table. 
    /// For the Current User's assigned ministries - maybe be 1+ ministry
    /// </summary>
    /// <returns>All Communication Contacts in a ministry the user is associated with</returns>
    public IQueryable<ActiveDistinctCommunicationContact> GetCommunicationContactsByCurrentUser(CommunicationSortOrder sort, CustomPrincipal customPrincipal)
    {
        // If user is in an 'ApplicationOwnerOrganizations' then we show them all
        // otherwise limit to the user's assigned ministries
        IQueryable<ActiveCommunicationContact> activecommcontacts = corporateCalendarDataContext.ActiveCommunicationContacts;
        if (!customPrincipal.IsInApplicationOwnerOrganizations)
        {
            IEnumerable<Guid> systemUserMinistries = customPrincipal.SystemUserMinistryIds;
            activecommcontacts = activecommcontacts.Where(p => systemUserMinistries.Contains(p.MinistryId));
        }


        var distinctcomm = corporateCalendarDataContext.ActiveDistinctCommunicationContacts;

        IQueryable<ActiveDistinctCommunicationContact> options = (from orig in distinctcomm
                                                                  from b in activecommcontacts
                                                                  where orig.SystemUserId == b.SystemUserId
                                                                  select orig).Distinct();

        if (sort == CommunicationSortOrder.RoleThenName)
        {
            options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);
        }
        else
        {
            options = options.OrderBy(p => p.Name);
        }

        return options;
    }

    public IQueryable<ActiveDistinctCommunicationContact> GetCommunicationContactByMinistryIdIncludingId(Guid ministryId, int? includedId, CommunicationSortOrder sort)
    {
        // restrict displayed contacts based on the ministry parameter
        var commcontactsandministry = corporateCalendarDataContext.CommunicationContacts
            .Where(p => p.MinistryId == ministryId && (p.IsActive == true || p.Id == includedId));

        // Get list of the distinct users.
        var systemUsers = corporateCalendarDataContext.SystemUsers;

        IQueryable<ActiveDistinctCommunicationContact> options = (from user in systemUsers
                                                                  from b in commcontactsandministry
                                                                  where user.Id == b.SystemUserId
                                                                  && (b.Id == includedId || user.IsActive)
                                                                  select new ActiveDistinctCommunicationContact
                                                                  {
                                                                      SortOrder = b.SortOrder,
                                                                      SystemUserId = user.Id,
                                                                      Name = b.Name,
                                                                      NameAndNumber = b.Name + " " + user.PhoneNumber
                                                                  }
                                                                 ).Distinct();

        if (sort == CommunicationSortOrder.RoleThenName)
        {
            options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);
        }
        else
        {
            options = options.OrderBy(p => p.Name);
        }

        return options;
    }
    #endregion

    #region Ministry 
    /// <summary>
    /// Returns active rows from the Ministry table and sorts by SortOrder column.
    /// The ministries returned are for non-application-owners
    /// </summary>
    /// <returns>System.Object</returns>
    public IQueryable<Ministry> GetActiveMinistryOptions(CustomPrincipal customPrincipal, List<Guid> includedMinistryIds = null)
    {
        var um = customPrincipal.SystemUserMinistryIds;

        IQueryable<Ministry> options = corporateCalendarDataContext.Ministries.Where(p => (um.Contains(p.Id) && p.IsActive) || (includedMinistryIds ?? noGuids).Contains(p.Id))
            .OrderBy(p => p.Abbreviation);

        return options;
    }
    #endregion

    #region All Ministries (for application owners)
    /// <summary>
    /// Returns active rows from the Ministry table and sorts by SortOrder column.
    /// The ministries returned are for non-applicationowners
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetAllActiveMinistryOptions(List<Guid> includedMinistryIds = null)
    {
        if (includedMinistryIds == null)
            includedMinistryIds = noGuids;
        IQueryable<Ministry> options = corporateCalendarDataContext.Ministries.Where(p => (p.IsActive && !p.DisplayName.StartsWith("Minister of State")) || includedMinistryIds.Contains(p.Id))
            .OrderBy(p => p.Abbreviation);

        return options;
    }
    #endregion

    #region City
    /// <summary>
    /// Returns active rows from the CommunicationContacts table. The NameAndNumber column is defined in the view.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveCityOptions(int? includedCityId = null, bool showLegacy = false)
    {
        IQueryable<City> options = corporateCalendarDataContext.Cities.Where(p => p.IsActive == true || p.Id == includedCityId);

        if (!showLegacy)
        {
            string[] legacyItems = {
            "TBD",
            "Provincewide",
            "Nationwide"
        };

            options = options.Where(e => !legacyItems.Contains(e.Name) || e.Id == includedCityId);
        }

        options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);

        return options;
    }
    #endregion

    #region Category
    /// <summary>
    /// Returns active rows from the Category table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveCategoryOptions(bool isHq, List<int> includedIds = null, bool showLegacy = false)
    {
        IQueryable<Category> options = corporateCalendarDataContext.Categories
            .Where(p => p.IsActive || (includedIds??noIds).Contains(p.Id) || (isHq && p.Name == "HQ Placeholder"));

        return options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);
    }
    #endregion

    #region Communication Materials
    /// <summary>
    /// Returns active rows from the CommunicationMaterial table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public IQueryable<CommunicationMaterial> GetActiveCommunicationMaterialsOptions(List<int> includedIds)
    {
        if (includedIds == null)
            includedIds = noIds;
        IQueryable<CommunicationMaterial> options = corporateCalendarDataContext.CommunicationMaterials.Where(p => p.IsActive == true || includedIds.Contains(p.Id));

        return options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);
    }

    #endregion

    #region NR Origin
    /// <summary>
    /// Returns active rows from the NROrigin table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveNrOriginOptions(List<int> includedIds)
    {
        if (includedIds == null)
            includedIds = noIds;
        IQueryable<NROrigin> options = corporateCalendarDataContext.NROrigins.Where(p => p.IsActive || includedIds.Contains(p.Id));

        return options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name).ToArray();
    }
    #endregion

    #region NR Distribution
    /// <summary>
    /// Returns active rows from the Region table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveNrDistributionOptions(int? includedId = null, bool showLegacy = false)
    {
        IQueryable<NRDistribution> options = corporateCalendarDataContext.NRDistributions.Where(p => p.IsActive || p.Id == includedId);

        if (!showLegacy)
        {
            string[] legacyItems =  {
            "–"
            };

            options = options.Where(e => !legacyItems.Contains(e.Name) || e.Id == includedId);
        }

        return options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);
    }
    #endregion

    #region Sector
    /// <summary>
    /// Returns active rows from the Sector table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveSectorOptions(List<Guid> includedIds)
    {
        IEnumerable<Sector> options = corporateCalendarDataContext.Sectors.Where(p => p.IsActive || (includedIds??noGuids).Contains(p.Id));

        return options.OrderBy(p => p.DisplayName).ToArray();
    }
    #endregion

    #region Theme
    /// <summary>
    /// Returns active rows from the Theme table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveThemeOptions(List<Guid> includedIds = null)
    {
        IQueryable<Theme> options = corporateCalendarDataContext.Themes.Where(p => p.IsActive || (includedIds??noGuids).Contains(p.Id));

        return options.OrderBy(p => p.SortOrder).ThenBy(p => p.DisplayName).ToArray();
    }
    #endregion

    #region Keyword
    /// <summary>
    /// Returns active rows from the Keyword table and sorts by Name.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveKeywordOptionsSecurely(int systemUserId, bool isCurrentUserInOwnerList, List<int> includedIds = null)
    {
        var ministries = corporateCalendarDataContext.SystemUserMinistries.Where(m => m.SystemUserId == systemUserId && m.IsActive).Select(m => m.MinistryId);
        var cutOffDate = DateTime.Today.AddDays(-90);

        var keywords = from a in corporateCalendarDataContext.ActiveActivities
                       join ak in corporateCalendarDataContext.ActivityKeywords on a.Id equals ak.ActivityId
                       join k in corporateCalendarDataContext.Keywords on ak.KeywordId equals k.Id
                       where (includedIds??noIds).Contains(k.Id) || 
                            k.IsActive
                            && (k.LastUpdatedDateTime >= cutOffDate &&
                            (isCurrentUserInOwnerList // User is HQ
                           || !a.IsConfidential // Activity is not confidential
                           || ministries.Any(m => m == a.ContactMinistryId) // User's Ministry
                           || ministries.Any(m => corporateCalendarDataContext.ActivitySharedWiths.Any(asw => asw.ActivityId == a.Id && asw.MinistryId == m))))  // Shared with User's Ministry
                       select k;

        return keywords.Distinct().OrderBy(k => k.SortOrder).ToArray();
    }
    #endregion

    #region Initiative
    /// <summary>
    /// Returns active rows from the Initiative table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveInitiativeOptions(List<int> includedIds = null)
    {
        IQueryable<Initiative> options = corporateCalendarDataContext.Initiatives.Where(p => p.IsActive || (includedIds??noIds).Contains(p.Id));

        return options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name).ToArray();
    }
    #endregion

    #region Premier Requested
    /// <summary>
    /// Returns active rows from the PremierRequested table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActivePremierRequestedOptions(int? includedId = null, bool showLegacy = false)
    {
        IQueryable<PremierRequested> options = corporateCalendarDataContext.PremierRequested.Where(p => p.IsActive || p.Id == includedId);

        if (!showLegacy)
        {
            string[] legacyItems = {
                "N/A",
            };

            options = options.Where(e => !legacyItems.Contains(e.Name));
        }

        options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);

        return options;
    }
    #endregion

    #region Event Planner
    /// <summary>
    /// Returns active rows from the PremierRequested table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveEventPlannerOptions(int? includedId = null, bool showLegacy = false)
    {
        IQueryable<EventPlanner> options = corporateCalendarDataContext.EventPlanners.Where(p => p.IsActive == true || p.Id == includedId);

        if (!showLegacy)
        {
            string[] legacyItems = {
                "–"
            };

            options = options.Where(e => !legacyItems.Contains(e.Name) || e.Id == includedId);
        }

        options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);

        return options;
    }
    #endregion

    #region Videographer
    /// <summary>
    /// Returns active rows from the PremierRequested table and sorts by SortOrder column.
    /// </summary>
    /// <returns>System.Object</returns>
    public object GetActiveVideographerOptions(int? includedId = null, bool showLegacy = false)
    {
        IQueryable<Videographer> options = corporateCalendarDataContext.Videographers.Where(p => p.IsActive == true || p.Id == includedId);

        if (!showLegacy)
        {
            string[] legacyItems = {
                "–"
            };

            options = options.Where(e => !legacyItems.Contains(e.Name) || e.Id == includedId);
        }

        options = options.OrderBy(p => p.SortOrder).ThenBy(p => p.Name);

        return options;
    }

    #endregion
}