CREATE FUNCTION [calendar].[sGetActivityNROrigins] (@ActivityId as int)

returns varchar(1000)
AS
BEGIN
DECLARE @names varchar(1000)

SELECT @names= COALESCE(@names+',' ,'') + ' ' + NROrigin.Name 
	FROM ActivityNROrigins 
	inner join NROrigin  on ActivityNROrigins.NROriginId = NROrigin.Id
	WHERE ActivityNROrigins.ActivityId = @ActivityId
return @names	
end