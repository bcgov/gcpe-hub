




CREATE VIEW [calendar].[ActiveCommunicationContacts]
AS
SELECT 
	TOP (100) PERCENT 
	cc.Id, cc.SystemUserId, 
	cc.Name COLLATE DATABASE_DEFAULT + ' ' + [calendar].SystemUser.PhoneNumber COLLATE DATABASE_DEFAULT AS NameAndNumber, 
	cc.MinistryShortName, 
	cc.Name, 
	[dbo].[Ministry].Id AS MinistryId, 
	cc.MinistryId AS Expr1,
	cc.SortOrder
FROM [calendar].CommunicationContact AS cc 
INNER JOIN [dbo].[Ministry] ON cc.MinistryShortName COLLATE DATABASE_DEFAULT = [dbo].[Ministry].Abbreviation COLLATE DATABASE_DEFAULT OR  cc.MinistryId = [dbo].[Ministry].Id 
INNER JOIN [calendar].SystemUser ON cc.SystemUserId = [calendar].SystemUser.Id

WHERE (cc.IsActive = 1) 
AND ([calendar].SystemUser.IsActive = 1)
AND [dbo].[Ministry].IsActive = 1
ORDER BY cc.SortOrder