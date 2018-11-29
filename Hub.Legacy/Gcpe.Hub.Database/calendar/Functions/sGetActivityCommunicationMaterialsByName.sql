-- =============================================
-- Author:		Ali
-- Create date: 09/10/2011
-- Description:	Function to return ActivitySharedWith comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[sGetActivityCommunicationMaterialsByName] (@ActID as int)

returns varchar(1000)
AS
BEGIN
DECLARE @catnames varchar(1000)

SELECT @catnames= COALESCE(@catnames+',' ,'') + ' ' + cm.Name
	FROM ActivityCommunicationMaterials ac
	INNER JOIN [calendar].CommunicationMaterial AS cm ON ac.CommunicationMaterialId = cm.Id
	WHERE ac.ActivityId = @ActID
	AND cm.IsActive = 1
return @catnames	
end
