

-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivitySharedWith comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[sGetActivityCategoriesByName] (@ActID as int)

returns varchar(1000)
AS
BEGIN
DECLARE @catnames varchar(1000)

SELECT @catnames= COALESCE(@catnames+',' ,'') + ' ' + c.Name --CAST([CategoryId] AS VARCHAR(100))
	FROM ActivityCategories ac
	inner join Category c on ac.CategoryId = c.Id
	WHERE ActivityId = @ActID
return @catnames	
end


