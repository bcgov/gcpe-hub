

-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivityNewsReleaseOrigin comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[GetActivityNewsReleaseOrigins]
(@activityId int)
RETURNS @activityNewsReleaseOriginsTable TABLE (activityId int, activityNewsReleaseOrigins VARCHAR(4096))
AS
BEGIN
	DECLARE @commaDelimitedString varchar(4096)

	SELECT @commaDelimitedString = COALESCE(@commaDelimitedString+',' ,'') + CAST([NROriginId] AS VARCHAR(100))
	FROM ActivityNROrigins
	WHERE ActivityId = @activityId
	
	INSERT INTO @activityNewsReleaseOriginsTable
	VALUES (@activityId,@commaDelimitedString)

	RETURN

END




