

-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivitySharedWith comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[sGetActivitySectorsByName] (@ActID as int)

returns varchar(1000)
AS
BEGIN
DECLARE @sectornames varchar(1000)

SELECT @sectornames = COALESCE(@sectornames +',' ,'') + ' ' + s.DisplayName 
	FROM ActivitySectors acs
	inner join [Sector] s on acs.SectorId = s.Id
	WHERE ActivityId = @ActID
return @sectornames	
end


