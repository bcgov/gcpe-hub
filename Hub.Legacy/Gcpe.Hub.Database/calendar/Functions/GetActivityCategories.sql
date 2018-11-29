
-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivityCategories comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[GetActivityCategories]
(@activityId int)
RETURNS @activityCategoryTable TABLE (activityId int, categories VARCHAR(4096))
AS
BEGIN
	DECLARE @commaDelimitedString varchar(4096)

	SELECT @commaDelimitedString = COALESCE(@commaDelimitedString+',' ,'') + CAST([CategoryId] AS VARCHAR(100))
	FROM ActivityCategories
	WHERE ActivityId = @activityId
	
	INSERT INTO @activityCategoryTable
	VALUES (@activityId,@commaDelimitedString)

	RETURN

END



