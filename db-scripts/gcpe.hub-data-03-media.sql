USE [Gcpe.Hub]
GO

INSERT [media].[MediaType] ([Id], [MediaTypeName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Radio', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 2);
INSERT [media].[MediaType] ([Id], [MediaTypeName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Print', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 3);
INSERT [media].[MediaType] ([Id], [MediaTypeName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'TV', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1);
INSERT [media].[MediaType] ([Id], [MediaTypeName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Web', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 4);
INSERT [media].[MediaType] ([Id], [MediaTypeName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Other', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 5);


INSERT [media].[PhoneType] ([Id], [PhoneTypeName], [SortOrder]) VALUES (NEWID(), N'After Hours', 6);
INSERT [media].[PhoneType] ([Id], [PhoneTypeName], [SortOrder]) VALUES (NEWID(), N'Primary', 1);
INSERT [media].[PhoneType] ([Id], [PhoneTypeName], [SortOrder]) VALUES (NEWID(), N'Pager', 5);
INSERT [media].[PhoneType] ([Id], [PhoneTypeName], [SortOrder]) VALUES (NEWID(), N'Cell', 2);
INSERT [media].[PhoneType] ([Id], [PhoneTypeName], [SortOrder]) VALUES (NEWID(), N'Fax', 3);
INSERT [media].[PhoneType] ([Id], [PhoneTypeName], [SortOrder]) VALUES (NEWID(), N'News Desk', 4);
INSERT [media].[PhoneType] ([Id], [PhoneTypeName], [SortOrder]) VALUES (NEWID(), N'Other', 7);

INSERT [media].[PrintCategory] ([Id], [PrintCategoryName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Journal', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 3);
INSERT [media].[PrintCategory] ([Id], [PrintCategoryName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Magazine', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 2);
INSERT [media].[PrintCategory] ([Id], [PrintCategoryName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Newspaper', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 1);

INSERT [media].[WebAddressType] ([Id], [WebAddressTypeName], [SortOrder]) VALUES (NEWID(), N'Google+ URL', 5);
INSERT [media].[WebAddressType] ([Id], [WebAddressTypeName], [SortOrder]) VALUES (NEWID(), N'Twitter Handle', 3);
INSERT [media].[WebAddressType] ([Id], [WebAddressTypeName], [SortOrder]) VALUES (NEWID(), N'Email Address', 1);
INSERT [media].[WebAddressType] ([Id], [WebAddressTypeName], [SortOrder]) VALUES (NEWID(), N'Facebook URL', 4);
INSERT [media].[WebAddressType] ([Id], [WebAddressTypeName], [SortOrder]) VALUES (NEWID(), N'Website URL', 2);

INSERT [media].[Beat] ([Id], [BeatName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Politics', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[ContactCity] ([Id], [CityName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Fredericton', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[MediaJobTitle] ([Id], [MediaJobTitleName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Editor', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[MediaJobTitle] ([Id], [MediaJobTitleName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Reporter', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[Country] ([Id], [CountryName], [CountryAbbrev], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Canada', N'CAD', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[ProvState] ([Id], [ProvStateName], [ProvStateAbbrev], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'New Brunswick', N'NB', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[MediaLanguage] ([Id], [LanguageName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'English', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[MediaLanguage] ([Id], [LanguageName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'French', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);

INSERT INTO [media].[MinisterialJobTitle]([Id],[MinisterialJobTitleName],[CreationDate],[ModifiedDate],[SortOrder]) VALUES (NEWID() ,'Minister' ,CURRENT_TIMESTAMP ,CURRENT_TIMESTAMP ,1)


GO

