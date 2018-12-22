USE [Gcpe.Hub]
GO

--
-- Need the role id in the gcpe.hub-data-03-systemuser script.
--
DECLARE @systemAdminRoleId int = 5;

SET IDENTITY_INSERT [calendar].[Role] ON 

INSERT [calendar].[Role] ([Id], [Name], [Description], [IsActive], [RowGuid]) VALUES (1, N'ReadOnly', N'User can view activities within their own ministry / organization.', 1, NEWID())

INSERT [calendar].[Role] ([Id], [Name], [Description], [IsActive], [RowGuid]) VALUES (2, N'Editor', N'User can add, edit, delete activities within their own ministry / organization.', 1, NEWID())

INSERT [calendar].[Role] ([Id], [Name], [Description], [IsActive], [RowGuid]) VALUES (3, N'Advanced', N'User can add, edit, delete activities across ministries  / organizations.', 1, NEWID())

INSERT [calendar].[Role] ([Id], [Name], [Description], [IsActive], [RowGuid]) VALUES (4, N'Administrator', N'User can add, edit, delete activities across ministries  / organizations, manage users and modify lookup tables', 1, NEWID())

INSERT [calendar].[Role] ([Id], [Name], [Description], [IsActive], [RowGuid]) VALUES (@systemAdminRoleId, N'SysAdmin', N'User can add, edit, delete activities across ministries  / organization, manage users, modify lookup tables and view restricted system information.', 1, NEWID())

SET IDENTITY_INSERT [calendar].[Role] OFF


SET IDENTITY_INSERT [calendar].[City] ON 

INSERT [calendar].[City] ([Id], [Name], [IsActive], [SortOrder]) VALUES (311, N'Other...', 1, 0)
INSERT [calendar].[City] ([Id], [Name], [IsActive], [SortOrder]) VALUES (322, N'Victoria, BC', 1, 0)

SET IDENTITY_INSERT [calendar].[City] OFF


SET IDENTITY_INSERT [calendar].[Status] ON 

INSERT [calendar].[Status] ([Id], [Name], [IsActive], [RowGuid]) VALUES (1, N'Changed', 1, NEWID())

INSERT [calendar].[Status] ([Id], [Name], [IsActive], [RowGuid]) VALUES (2, N'Reviewed', 1, NEWID())

INSERT [calendar].[Status] ([Id], [Name], [IsActive], [RowGuid]) VALUES (7, N'New', 1, NEWID())

SET IDENTITY_INSERT [calendar].[Status] OFF


SET IDENTITY_INSERT [calendar].[PremierRequested] ON 

INSERT [calendar].[PremierRequested] ([Id], [Name], [IsActive], [SortOrder]) VALUES (1, N'Yes', 1, 10)

INSERT [calendar].[PremierRequested] ([Id], [Name], [IsActive], [SortOrder]) VALUES (2, N'No', 1, 15)

INSERT [calendar].[PremierRequested] ([Id], [Name], [IsActive], [SortOrder]) VALUES (3, N'Premier Not Available', 1, 40)

INSERT [calendar].[PremierRequested] ([Id], [Name], [IsActive], [SortOrder]) VALUES (4, N'Premier Confirmed', 1, 25)

INSERT [calendar].[PremierRequested] ([Id], [Name], [IsActive], [SortOrder]) VALUES (5, N'N/A', 1, 5)

INSERT [calendar].[PremierRequested] ([Id], [Name], [IsActive], [SortOrder]) VALUES (6, N'Premier TBC', 1, 20)

SET IDENTITY_INSERT [calendar].[PremierRequested] OFF
GO


SET IDENTITY_INSERT [calendar].[Category] ON 
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (60, N'HQ Placeholder', 1, 0, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (1, N'Marketing / Advertising', 84, 0, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (2, N'Awareness Day / Week / Month', 20, 1 ,NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (3, N'Conference / AGM / Forum', 23, 1, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (9, N'FYI Only', 24, 1, NEWID())
INSERT [calendar].[Category] ([Id], [Name] ,[SortOrder] ,[IsActive] ,[RowGuid]) VALUES (12, 'Proposed Release' ,26 ,1 ,NEWID())
INSERT [calendar].[Category] ([Id], [Name] ,[SortOrder] ,[IsActive] ,[RowGuid]) VALUES (58, 'Approved Release' ,11 ,1 ,NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (59, N'Approved Event or Activity', 10, 1, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (55, N'Proposed Event or Activity', 25, 1, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (16, N'Speech /  Remarks', 140, 1, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (18, N'TV / Radio', 173, 1, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (51, N'IGRS use: Half-Masting', 195, 1, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (52, N'IGRS use: National Day', 205, 1, NEWID())
INSERT [calendar].[Category] ([Id], [Name], [SortOrder], [IsActive], [RowGuid]) VALUES (53, N'IGRS use: Visit', 215, 1, NEWID())

SET IDENTITY_INSERT [calendar].[Category] OFF
GO

SET IDENTITY_INSERT [calendar].[NROrigin] ON 

INSERT [calendar].[NROrigin] ([Id], [Name], [IsActive], [SortOrder], [RowGuid]) VALUES (3, N'Ministry', 1, 15,  NEWID())
INSERT [calendar].[NROrigin] ([Id], [Name], [IsActive], [SortOrder], [RowGuid]) VALUES (5, N'Joint Prov/Fed release', 1, 25, NEWID())
INSERT [calendar].[NROrigin] ([Id], [Name], [IsActive], [SortOrder], [RowGuid]) VALUES (7, N'3rd Party release', 1, 30, NEWID())
INSERT [calendar].[NROrigin] ([Id], [Name], [IsActive], [SortOrder], [RowGuid]) VALUES (28, N'Joint Prov/3rd party release', 1, 20,  NEWID())

SET IDENTITY_INSERT [calendar].[NROrigin] OFF
GO