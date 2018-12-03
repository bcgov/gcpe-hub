USE [Gcpe.Hub]
GO

-- See the gcpe.hub-data-01-dbo script.
DECLARE @hqMinistryId uniqueidentifier = N'321d9c68-96ba-417b-bb89-78c7ce275458'; 
DECLARE @hqMinistryShortName varchar(50) = N'GCPEHQ';


-- see gcpe.hub-data-02-calendar script.
DECLARE @systemAdminRoleId int = 5;

--
-- set to the LDAP User account name of the system administrator
--
DECLARE @systemUsername nvarchar(20) = N'<ENTER_SYSTEM_USERNAME';
DECLARE @fullName nvarchar(100) = N'<ENTER_USER_FULLNAME>';
DECLARE @systemUserId int = 1; 

SET IDENTITY_INSERT [calendar].[SystemUser] ON 

INSERT [calendar].[SystemUser] ([Id], [Username], [RoleId], [Description], [FullName], [DisplayName], [JobTitle], [PhoneNumber], [MobileNumber], [EmailAddress], [FilterDisplayValue], [IsActive], [CreatedDateTime], [CreatedBy], [LastUpdatedDateTime], [LastUpdatedBy], [HiddenColumns]) VALUES (@systemUserId, @systemUsername, @systemAdminRoleId, NULL, N'', N'', N'', N'', N'', N'', 1, 1, NULL, NULL, NULL, NULL, N'')

SET IDENTITY_INSERT [calendar].[SystemUser] OFF



SET IDENTITY_INSERT [calendar].[SystemUserMinistry] ON 

INSERT [calendar].[SystemUserMinistry] ([Id], [SystemUserId], [MinistryId], [IsActive]) VALUES (1, @systemUserId, @hqMinistryId, 1)

SET IDENTITY_INSERT [calendar].[SystemUserMinistry] OFF


SET IDENTITY_INSERT [calendar].[CommunicationContact] ON 

INSERT [calendar].[CommunicationContact] ([Id], [SystemUserId], [Name], [MinistryShortName], [MinistryId], [SortOrder], [IsActive]) VALUES (1, @systemUserId, @fullName, @hqMinistryShortName, @hqMinistryId, 1, 1)

SET IDENTITY_INSERT [calendar].[CommunicationContact] OFF
GO
