

-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivitySharedWith comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[GetActivitySharedWithMinistries]
(@activityId int)
RETURNS @activitySharedWithTable TABLE (activityId int, sharedWithMinistries VARCHAR(4096))
AS
BEGIN
	DECLARE @commaDelimitedString varchar(4096)

	SELECT @commaDelimitedString = COALESCE(@commaDelimitedString+',' ,'') + CAST([MinistryId] AS VARCHAR(50))
	FROM ActivitySharedWith
	WHERE ActivityId = @activityId
	
	INSERT INTO @activitySharedWithTable
	VALUES (@activityId,@commaDelimitedString)

	RETURN

END




