USE [Gcpe.Hub]
GO

INSERT [media].[Distribution] ([Id], [DistributionName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Sample Distribution', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[ElectoralDistrict] ([Id], [DistrictName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Sample Electorial District', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[Ethnicity] ([Id], [EthnicityName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Sample Ethnicity', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[MediaDesk] ([Id], [MediaDeskName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Sample Media Desk', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[NewsRegion] ([Id], [RegionName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Sample News Region', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[PublicationDays] ([Id], [PublicationDaysName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Sample Publication Day', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[PublicationFrequency] ([Id], [PublicationFrequencyName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Sample Publication Frequency', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);
INSERT [media].[SpecialtyPublication] ([Id], [SpecialtyPublicationName], [CreationDate], [ModifiedDate], [SortOrder]) VALUES (NEWID(), N'Sample Specialty Publication', CURRENT_TIMESTAMP, CURRENT_TIMESTAMP, 999);

GO