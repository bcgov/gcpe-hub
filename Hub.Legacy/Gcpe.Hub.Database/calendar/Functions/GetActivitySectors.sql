

-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivitySector comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[GetActivitySectors]
(@activityId int)
RETURNS @activitySectorsTable TABLE (activityId int, activitySectors VARCHAR(4096))
AS
BEGIN
	DECLARE @commaDelimitedString varchar(4096)

	SELECT @commaDelimitedString = COALESCE(@commaDelimitedString+',' ,'') + CAST([SectorId] AS VARCHAR(100))
	FROM ActivitySectors
	WHERE ActivityId = @activityId
	
	INSERT INTO @activitySectorsTable
	VALUES (@activityId,@commaDelimitedString)

	RETURN

END




