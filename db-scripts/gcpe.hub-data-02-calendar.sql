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

INSERT INTO [calendar].[Category] ([Name] ,[SortOrder] ,[IsActive] ,[RowGuid]) VALUES ('Awareness Day / Week / Month' ,20 ,1 ,NEWID())
GO