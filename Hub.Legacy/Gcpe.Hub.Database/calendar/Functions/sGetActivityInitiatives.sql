

CREATE FUNCTION [calendar].[sGetActivityInitiatives] (@ActivityId as int)

returns varchar(1000)
AS
BEGIN
DECLARE @commaDelimitedString varchar(1000)

SELECT @commaDelimitedString= COALESCE(@commaDelimitedString+',' ,'') + CAST([InitiativeId] AS VARCHAR(10)) 
	FROM ActivityInitiatives
	WHERE ActivityInitiatives.ActivityId = @ActivityId
return @commaDelimitedString	
end