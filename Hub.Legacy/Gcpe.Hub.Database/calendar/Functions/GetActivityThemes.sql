

CREATE FUNCTION [calendar].[GetActivityThemes]
(@activityId int)
RETURNS @activityThemesTable TABLE (activityId int, activityThemes VARCHAR(4096))
AS
BEGIN
	DECLARE @commaDelimitedString varchar(4096)

	SELECT @commaDelimitedString = COALESCE(@commaDelimitedString+',' ,'') + CAST([ThemeId] AS VARCHAR(100))
	FROM ActivityThemes
	WHERE ActivityId = @activityId
	
	INSERT INTO @activityThemesTable
	VALUES (@activityId,@commaDelimitedString)

	RETURN

END




