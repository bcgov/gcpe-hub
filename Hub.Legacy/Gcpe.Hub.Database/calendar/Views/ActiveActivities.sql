CREATE VIEW [calendar].[ActiveActivities]
AS
SELECT      [calendar].Activity.Id, [calendar].Activity.StatusId, [calendar].Activity.HqStatusId, [calendar].Activity.StartDateTime, [calendar].Activity.EndDateTime, [calendar].Activity.IsConfirmed, 
            [calendar].Activity.PotentialDates, [calendar].Activity.Title, [calendar].Activity.Details, [calendar].Activity.Comments, [calendar].Activity.HqComments, [calendar].Activity.HqSection, 
            [calendar].Activity.PremierRequestedId, [calendar].Activity.ContactMinistryId, [dbo].[Ministry].Abbreviation AS Ministry, [calendar].Activity.LeadOrganization, [calendar].Activity.CityId, 
            [calendar].Activity.Venue, [calendar].Activity.GovernmentRepresentativeId, [calendar].Activity.IsAllDay, [calendar].Activity.IsAtLegislature, [calendar].Activity.IsConfidential, 
            [calendar].Activity.IsCrossGovernment, [calendar].Activity.IsActive,[calendar].Activity.Schedule, [calendar].Activity.Significance, [calendar].Activity.IsIssue,
            [calendar].Activity.IsMilestone, [calendar].Activity.Strategy, [calendar].Activity.NRDateTime, [calendar].Activity.CreatedDateTime, [calendar].Activity.CreatedBy,
            [calendar].Activity.LastUpdatedDateTime, [calendar].Activity.LastUpdatedBy, [calendar].Activity.CommunicationContactId, [calendar].Status.Name AS Status, HqStatus.Name AS HqStatus,
            [calendar].City.Name AS City, [calendar].Activity.Translations, [calendar].Activity.NRDistributionId, [calendar].sGetActivitySharedWithMinistries([calendar].Activity.Id) AS SharedWithMinistryIds, 
            [calendar].sGetActivityFavoriteSystemUsers([calendar].Activity.Id) AS FavoriteSystemUsers,[calendar].sGetActivityCategoriesByName([calendar].Activity.Id) AS Categories,
            [calendar].sGetActivitySectorsByName([calendar].Activity.Id) AS Sectors, [calendar].sGetActivityNROrigins([calendar].Activity.Id) AS NROrigins, 
            STUFF((Select ', '+ keywordName FROM [calendar].GetActivityKeywords([calendar].Activity.Id) FOR XML PATH('')), 1, 1, '') AS Keywords,
            [calendar].NRDistribution.Name AS NRDistributions, [calendar].GovernmentRepresentative.Name AS GovernmentRepresentative, [calendar].Activity.OtherCity, 
            [calendar].PremierRequested.Name AS PremierRequested, [calendar].CommunicationContact.Name AS ContactName, [calendar].SystemUser.PhoneNumber AS ContactNumber,
            [calendar].Activity.EventPlannerId, [calendar].Activity.VideographerId, [calendar].sGetActivityCommunicationMaterialsByName([calendar].Activity.Id) AS CommunicationsMaterials, 
            [calendar].Activity.IsTitleNeedsReview, [calendar].Activity.IsDetailsNeedsReview, [calendar].Activity.IsRepresentativeNeedsReview, [calendar].Activity.IsCityNeedsReview, 
            [calendar].Activity.IsStartDateNeedsReview, [calendar].Activity.IsEndDateNeedsReview, [calendar].Activity.IsCategoriesNeedsReview, [calendar].Activity.IsActiveNeedsReview, [calendar].Activity.IsCommMaterialsNeedsReview,
            [calendar].Activity.IsSignificanceNeedsReview, [calendar].Activity.IsStrategyNeedsReview, [calendar].Activity.IsInternalNotesNeedsReview, [calendar].Activity.IsSchedulingConsiderationsNeedsReview

FROM        [calendar].Activity LEFT OUTER JOIN
            [calendar].Status ON [calendar].Activity.StatusId = [calendar].Status.Id LEFT OUTER JOIN
            [calendar].Status AS HqStatus ON [calendar].Activity.HqStatusId = HqStatus.Id LEFT OUTER JOIN
            [calendar].City ON [calendar].Activity.CityId = [calendar].City.Id LEFT OUTER JOIN
            [calendar].PremierRequested ON [calendar].Activity.PremierRequestedId = [calendar].PremierRequested.Id LEFT OUTER JOIN
            [calendar].GovernmentRepresentative ON [calendar].Activity.GovernmentRepresentativeId = [calendar].GovernmentRepresentative.Id LEFT OUTER JOIN
            [calendar].NRDistribution ON [calendar].Activity.NRDistributionId = [calendar].NRDistribution.Id LEFT OUTER JOIN
            [calendar].CommunicationContact ON [calendar].Activity.CommunicationContactId = [calendar].CommunicationContact.Id INNER JOIN
            [calendar].SystemUser ON [calendar].SystemUser.Id = [calendar].CommunicationContact.SystemUserId LEFT OUTER JOIN
            [calendar].EventPlanner AS EventPlanner ON [calendar].Activity.EventPlannerId = EventPlanner.Id LEFT OUTER JOIN
            [calendar].Videographer AS Videographer ON [calendar].Activity.VideographerId = Videographer.Id INNER JOIN
            [dbo].[Ministry] ON [calendar].Activity.ContactMinistryId = [dbo].[Ministry].Id
