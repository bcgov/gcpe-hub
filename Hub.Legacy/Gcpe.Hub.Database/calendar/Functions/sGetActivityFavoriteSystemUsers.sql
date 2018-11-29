


-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivitySharedWith comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[sGetActivityFavoriteSystemUsers] (@ActID as int)

returns varchar(1000)
AS
BEGIN
DECLARE @sharedids varchar(1000)

SELECT @sharedids = COALESCE(@sharedids +',' ,'') + CAST([SystemUserId] AS VARCHAR(10))
	FROM FavoriteActivity
	WHERE ActivityId = @ActID
	
return @sharedids
end