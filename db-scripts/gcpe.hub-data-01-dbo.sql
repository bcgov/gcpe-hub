USE [Gcpe.Hub]
GO

-- See the gcpe.hub-data-01-dbo-required script.
DECLARE @hqMinistryId uniqueidentifier = N'321d9c68-96ba-417b-bb89-78c7ce275458'; 
DECLARE @hqMinistryShortName varchar(10) = N'GCPEHQ';
DECLARE @hqMinistryDisplayName varchar(255) = N'GCPE HQ';


INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (@hqMinistryId, @hqMinistryId, 180, @hqMinistryDisplayName, @hqMinistryShortName, 1, N'', N'', N'', CAST(N'2016-10-27T19:30:07.973' AS DateTime), N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL, N'', N'', N'', NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, N'')

INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (N'768dbf29-89c6-48d1-901e-017a8a3557a4', N'768DBF29-89C6-48D1-901E-017A8A3557A4', 190, N'GCPE Media Relations', N'GCPEMEDIA', 1, N'', N'', N'', CAST(N'2016-10-27T19:30:07.973' AS DateTime), N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL, N'', N'', N'', NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, N'')

INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (N'aae302f1-05e1-4df9-8df3-1e4dd557278d', N'office-of-the-premier', 0, N'Office of the Premier', N'PREM', 1, N'', N'', N'', CAST(N'2016-10-27T19:30:07.973' AS DateTime), N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL, N'', N'', N'', NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, N'')

INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (N'e6177ccb-93ec-4ab5-a75c-f795337a39cf', N'E6177CCB-93EC-4AB5-A75C-F795337A39CF', 1, N'Citizen Engagement', N'CITENG', 1, N'', N'', N'', CAST(N'2016-10-27T19:30:07.973' AS DateTime), N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL, N'', N'', N'', NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, N'')

INSERT [dbo].[Sector] ( [Key], [SortOrder], [IsActive], [DisplayName], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId]) VALUES ( N'government-operations', 0, 1, N'Government Operations', CURRENT_TIMESTAMP, N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL)

INSERT [dbo].[Theme] ([Key], [DisplayName], [Timestamp], [TopReleaseId], [FeatureReleaseId], [SortOrder], [IsActive]) VALUES (N'health', N'Health', CURRENT_TIMESTAMP, NULL, NULL, 0, 1)

INSERT [dbo].[Service] ([Key], [DisplayName], [SortOrder], [IsActive]) VALUES (N'municipalities', N'Municipalities', 0, 1)

INSERT [dbo].[Language] ([Id], [SortOrder], [Name]) VALUES (3084, 1, N'French')
INSERT [dbo].[Language] ([Id], [SortOrder], [Name]) VALUES (4105, 0, N'English')

INSERT [dbo].[MediaDistributionList] ([Id], [Key], [DisplayName], [SortOrder], [IsActive]) VALUES (NEWID(), N'budget', N'BUDGET', 0, 1);

INSERT [dbo].[NewsReleaseCollection] ([Id], [Name]) VALUES (N'42c50941-64db-41b7-8fda-94d7f7aa9ae1', N'2009-2013')
INSERT [dbo].[NewsReleaseCollection] ([Id], [Name]) VALUES (N'37e637e3-0a3a-45f7-82f1-feb94dcef00a', N'2013-2017')
INSERT [dbo].[NewsReleaseCollection] ([Id], [Name]) VALUES (N'42ae17e6-5a27-4f76-ba10-2d42dbc1c358', N'2017-2017')
INSERT [dbo].[NewsReleaseCollection] ([Id], [Name]) VALUES (N'dd1be731-8c9a-4f50-953d-b8db37f85d17', N'2017-2021')

-- insert ministry sectors...
INSERT INTO [dbo].[MinistrySector]
	  SELECT m.id, s.id from dbo.Ministry m, dbo.Sector s;

-- insert ministry language here... just seed with english... (4105)
insert into dbo.MinistryLanguage
  select m.id, 4105, m.DisplayName from dbo.Ministry m;

-- insert sector language here... just seed with english... (4105)
insert into dbo.SectorLanguage
  select s.id, 4105,s.DisplayName from dbo.Sector s;
  
GO

DECLARE @list varchar(8000)
DECLARE @pos INT
DECLARE @len INT
DECLARE @value varchar(8000)

declare @id uniqueidentifier
declare @key nvarchar(100)
declare @sortOrder int
declare @displayName nvarchar(255)
declare @abbrev nvarchar(10)

SET @list = 'Department of Agriculture-AGRI|Department of Community Services-COMMSRV|Department of Economic and Rural Development and Tourism-ERDT|Department of Education and Early Childhood Development-EECD|Department of Energy-ENERGY|Department of Environment-ENVIRO|Department of Finance-FIN|Department of Fisheries and Aquaculture-FISHAQUA|Department of Health and Wellness-HEALTH|Department of Intergovernmental Affairs-INTERGOV|Department of Justice-JUSTICE|Department of Labour and Workforce Development-LABOUR|Department of Natural Resources-NATURALRES|Department of Public Service Commission-PUBSRV|Department of Seniors-SENIORS|Department of Service Nova Scotia and Municipal Relations-NSMUNICIPAL|Department of Tourism, Culture and Heritage-TOURISM|Department of Transportation and Infrastructure Renewal-TRANSPO|'

set @pos = 0
set @len = 0
set @sortOrder = 10

WHILE CHARINDEX('|', @list, @pos+1)>0
BEGIN
    set @len = CHARINDEX('|', @list, @pos+1) - @pos
    set @value = SUBSTRING(@list, @pos, @len)

	declare @l int = CHARINDEX('-', @value, 1)
	
	set @id = NEWID()
	set @displayName = SUBSTRING(@value, 1, @l-1)
	set @abbrev = SUBSTRING(@value, @l+1, 10)
	set @key = REPLACE(LOWER(@value), ' ', '-')
    
	/*        
    PRINT @id -- for debug porpose   
	PRINT @displayName
	PRINT @abbrev
	PRINT @key
	PRINT ''
    */
    INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (@id, @key, @sortOrder, @displayName, @abbrev, 1, N'', N'', N'', CURRENT_TIMESTAMP, N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL, N'', N'', N'', NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, N'');
    -- add with English language...
    INSERT [dbo].[MinistryLanguage] ([MinistryId], [LanguageId], [Name]) VALUES (@id, 4105, @displayName);
    -- add sectors for the ministry...
    INSERT INTO [dbo].[MinistrySector]
	  SELECT @id, s.id FROM [dbo].[Sector] s



    set @pos = CHARINDEX('|', @list, @pos+@len) +1
	set @sortOrder = @sortOrder + 1
END

GO