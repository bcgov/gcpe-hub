USE [Gcpe.Hub]
GO

-- See the gcpe.hub-data-01-dbo-required script.
DECLARE @hqMinistryId uniqueidentifier = N'321d9c68-96ba-417b-bb89-78c7ce275458'; 
DECLARE @hqMinistryShortName varchar(10) = N'GCPEHQ';
DECLARE @hqMinistryDisplayName varchar(255) = N'GCPE HQ';


INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (@hqMinistryId, @hqMinistryId, 180, @hqMinistryDisplayName, @hqMinistryShortName, 1, N'', N'', N'', CAST(N'2016-10-27T19:30:07.973' AS DateTime), N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL, N'', N'', N'', NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, N'')

INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (N'768dbf29-89c6-48d1-901e-017a8a3557a4', N'768DBF29-89C6-48D1-901E-017A8A3557A4', 190, N'GCPE Media Relations', N'GCPEMEDIA', 1, N'', N'', N'', CAST(N'2016-10-27T19:30:07.973' AS DateTime), N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL, N'', N'', N'', NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, N'')

INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (N'aae302f1-05e1-4df9-8df3-1e4dd557278d', N'office-of-the-premier', 0, N'Office of the Premier', N'PREM', 1, N'<a href="mailto: premier@gov.bc.ca ">premier@gov.bc.ca </a>', N'http://www2.gov.bc.ca/assets/gov/government/ministries-organizations/premier-cabinet-mlas/minister-small/_horgan_small.jpg', N'<p>John Horgan was elected MLA for the new riding of Langford- Juan de Fuca in 2017. He was first elected to the legislature in 2005 as MLA for Juan de Fuca, and has been re-elected three times. He became leader of the BCNDP in 2014.</p><p>A husband and father, John and his wife Ellie have two grown sons. John and his siblings were raised in Saanich by his mother, after his father passed away when he was very young. John grew up playing sports and was a natural leader at school, on the basketball court and in the lacrosse box.</p><p>As a young man John worked in a B.C. mill and in a joinery. He earned his bachelor&rsquo;s and master&rsquo;s degrees at Trent University in Ontario and Sydney University in Australia. A meeting with Tommy Douglas sparked his interest in social democracy, and he went on to work for members of parliament in Ottawa.&nbsp;</p><p>A cancer survivor, he is a passionate advocate for health care, public schools and community services for families.</p>', CAST(N'2018-03-08T15:02:51.943' AS DateTime), N'<ul>
<li>
<a href="http://www.newsroom.gov.bc.ca/ministries/office-of-the-premier/speeches/"><span class="menu-header">Speeches</a></span></li>
</ul>  ', N'', N'@BCGovNews', N'https://www.flickr.com/photos/bcgovphotos/albums/72157683691437844', N'https://www.youtube.com/playlist?playnext=1&list=PLbER4Sxdn0R5vNUfhfiPq67HrC5toinSU', N'https://soundcloud.com/bcgov/sets/premier-1', N'<script src="//connect.facebook.net/en_US/all.js#xfbml=1"></script><fb:like-box href="https://www.facebook.com/BCProvincialGovernment" width="235" show_faces="false" stream="true" header="true" height="395"></fb:like-box>', N'<iframe width="235" src="https://www.youtube.com/embed/videoseries?list=PLbER4Sxdn0R5vNUfhfiPq67HrC5toinSU" frameborder="0" allowfullscreen></iframe>', N'<iframe width="235px" height="350" scrolling="no" frameborder="no" src="https://w.soundcloud.com/player/?url=https%3A//api.soundcloud.com/playlists/339102085&amp;auto_play=false&amp;hide_related=false&amp;show_comments=true&amp;show_user=true&amp;show_reposts=false&amp;visual=true"></iframe>', NULL, NULL, N'PO BOX 9041</br>
STN PROV GOVT</br>
VICTORIA, BC</br>
V8W 9E1</br> 
Telephone: 250-387-1715</br> 
Fax: 250-387-0087</br>', N'Premier John Horgan', N'Premier John Horgan', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'')

INSERT [dbo].[Ministry] ([Id], [Key], [SortOrder], [DisplayName], [Abbreviation], [IsActive], [MinisterEmail], [MinisterPhotoUrl], [MinisterPageHtml], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId], [MinisterAddress], [MinisterName], [MinisterSummary], [MinistryUrl], [ParentId], [ContactUserId], [SecondContactUserId], [WeekendContactNumber], [EodFinalizedDateTime], [EodLastRunUserId], [EodLastRunDateTime], [DisplayAdditionalName]) VALUES (N'e6177ccb-93ec-4ab5-a75c-f795337a39cf', N'E6177CCB-93EC-4AB5-A75C-F795337A39CF', 1, N'Citizen Engagement', N'CITENG', 1, N'', N'', N'Gov.bc.ca content is being updated to reflect the Government of B.C.’s recent change of leadership. For questions about Government of B.C. programs and services, please <a href="http://www2.gov.bc.ca/gov/content/home/contact-us">contact us</a>.', CAST(N'2016-10-27T19:31:07.973' AS DateTime), N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL, N'', N'', N'', NULL, NULL, NULL, NULL, N'', NULL, NULL, NULL, N'')


INSERT [dbo].[Sector] ( [Key], [SortOrder], [IsActive], [DisplayName], [Timestamp], [MiscHtml], [MiscRightHtml], [TwitterUsername], [FlickrUrl], [YoutubeUrl], [AudioUrl], [FacebookEmbedHtml], [YoutubeEmbedHtml], [AudioEmbedHtml], [TopReleaseId], [FeatureReleaseId]) VALUES ( N'sample-sector-key', 999, 1, N'Sample Sector', CURRENT_TIMESTAMP, N'', N'', N'', N'', N'', N'', N'', N'', N'', NULL, NULL)

INSERT [dbo].[Theme] ([Key], [DisplayName], [Timestamp], [TopReleaseId], [FeatureReleaseId], [SortOrder], [IsActive]) VALUES (N'sample-theme-key', N'Sample Theme', CURRENT_TIMESTAMP, NULL, NULL, 999, 1)

INSERT [dbo].[Service] ([Key], [DisplayName], [SortOrder], [IsActive]) VALUES (N'sample-service-key', N'Sample Service', 999, 1)

INSERT [dbo].[Language] ([Id], [SortOrder], [Name]) VALUES (3084, 1, N'French')
INSERT [dbo].[Language] ([Id], [SortOrder], [Name]) VALUES (4105, 0, N'English')

INSERT [dbo].[MediaDistributionList] ([Id], [Key], [DisplayName], [SortOrder], [IsActive]) VALUES (NEWID(), N'sample-media', N'Sample Media Distribution List', 999, 1);

INSERT [dbo].[NewsReleaseCollection] ([Id], [Name]) VALUES (N'42c50941-64db-41b7-8fda-94d7f7aa9ae1', N'2009-2013')
INSERT [dbo].[NewsReleaseCollection] ([Id], [Name]) VALUES (N'37e637e3-0a3a-45f7-82f1-feb94dcef00a', N'2013-2017')
INSERT [dbo].[NewsReleaseCollection] ([Id], [Name]) VALUES (N'42ae17e6-5a27-4f76-ba10-2d42dbc1c358', N'2017-2017')
INSERT [dbo].[NewsReleaseCollection] ([Id], [Name]) VALUES (N'dd1be731-8c9a-4f50-953d-b8db37f85d17', N'2017-2021')

GO