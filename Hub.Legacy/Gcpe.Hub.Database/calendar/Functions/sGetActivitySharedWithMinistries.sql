

-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivitySharedWith comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[sGetActivitySharedWithMinistries] (@ActID as int)

returns varchar(1500)
AS
BEGIN
DECLARE @sharedids varchar(1500)

SELECT @sharedids = COALESCE(@sharedids +',' ,'') + CAST([MinistryId] AS VARCHAR(50))
	FROM ActivitySharedWith
	WHERE ActivityId = @ActID
	
return @sharedids
end


