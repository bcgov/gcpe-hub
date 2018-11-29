


CREATE FUNCTION [calendar].[GetActivityKeywords]
(@activityId int)
RETURNS @activityKeywordsTable TABLE (keywordId int, keywordName VARCHAR(50))
AS
BEGIN
	INSERT INTO @activityKeywordsTable
	SELECT Id, Name
	FROM ActivityKeywords
	INNER JOIN Keyword on ActivityKeywords.KeywordId = Keyword.Id
	WHERE ActivityId = @activityId

	RETURN

END