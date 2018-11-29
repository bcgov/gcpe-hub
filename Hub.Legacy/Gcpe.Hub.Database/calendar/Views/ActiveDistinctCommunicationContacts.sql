

CREATE VIEW [calendar].[ActiveDistinctCommunicationContacts]
AS
SELECT 
	DISTINCT
	au.Id as SystemUserId,
	au.FullName COLLATE DATABASE_DEFAULT 
		+ ' ' + au.PhoneNumber COLLATE DATABASE_DEFAULT 
		as NameAndNumber
	,au.FullName as Name
	,cc.SortOrder
FROM [calendar].SystemUser au 
INNER JOIN [calendar].CommunicationContact AS cc ON cc.SystemUserId = au.Id 
WHERE (au.IsActive = 1)
AND (cc.IsActive = 1)

GO







GO


