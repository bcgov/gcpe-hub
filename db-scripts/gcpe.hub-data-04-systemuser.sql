USE [Gcpe.Hub]
GO

DECLARE @list varchar(8000)
DECLARE @pos INT
DECLARE @len INT
DECLARE @value varchar(8000)

-- See the gcpe.hub-data-01-dbo script.
DECLARE @hqMinistryId uniqueidentifier = N'321d9c68-96ba-417b-bb89-78c7ce275458'; 
DECLARE @hqMinistryShortName varchar(50) = N'GCPEHQ';

-- see gcpe.hub-data-02-calendar script.
DECLARE @systemAdminRoleId int = 5;

-- ensure there is a delimter at the end of the list...
-- list should be AD_ACCOUNT_NAME - Full Name|AD_ACCOUNT_NAME_2 - Full Name| ... AD_ACCOUNT_NAME_N - Full Name|
SET @list = ''

DECLARE @systemUsername nvarchar(20)
DECLARE @fullName nvarchar(100)
declare @systemUserId int

set @pos = 0
set @len = 0

WHILE CHARINDEX('|', @list, @pos+1)>0
BEGIN
    set @len = CHARINDEX('|', @list, @pos+1) - @pos
    set @value = SUBSTRING(@list, @pos, @len)

	declare @l int = CHARINDEX('-', @value, 1)
	
	set @systemUsername = SUBSTRING(@value, 1, @l-1)
	set @fullName = SUBSTRING(@value, @l+1, 100)
    
	/*        
    PRINT @@systemUsername -- for debug porpose   
	PRINT @@fullName
	PRINT ''
    */

	INSERT [calendar].[SystemUser] ([Username], [RoleId], [Description], [FullName], [DisplayName], [JobTitle], [PhoneNumber], [MobileNumber], [EmailAddress], [FilterDisplayValue], [IsActive], [CreatedDateTime], [CreatedBy], [LastUpdatedDateTime], [LastUpdatedBy], [HiddenColumns]) VALUES (@systemUsername, @systemAdminRoleId, NULL, N'', N'', N'', N'', N'', N'', 1, 1, NULL, NULL, NULL, NULL, N'')
	set @systemUserId = SCOPE_IDENTITY()

	INSERT [calendar].[SystemUserMinistry] ([SystemUserId], [MinistryId], [IsActive]) VALUES (@systemUserId, @hqMinistryId, 1)

	INSERT [calendar].[CommunicationContact] ([SystemUserId], [Name], [MinistryShortName], [MinistryId], [SortOrder], [IsActive]) VALUES (@systemUserId, @fullName, @hqMinistryShortName, @hqMinistryId, 1, 1)

	
	set @pos = CHARINDEX('|', @list, @pos+@len) +1

END
GO