

-- =============================================
-- Author:		Nick Pouliot
-- Create date: 09/10/2011
-- Description:	Function to return ActivityCommunicationMaterial comma delimited string
-- =============================================
CREATE FUNCTION [calendar].[GetActivityCommunicationMaterials]
(@activityId int)
RETURNS @activityCommunicationMaterialTable TABLE (activityId int, activityCommunicationMaterials VARCHAR(4096))
AS
BEGIN
	DECLARE @commaDelimitedString varchar(4096)

	SELECT @commaDelimitedString = COALESCE(@commaDelimitedString+',' ,'') + CAST([CommunicationMaterialId] AS VARCHAR(100))
	FROM ActivityCommunicationMaterials
	WHERE ActivityId = @activityId
	
	INSERT INTO @activityCommunicationMaterialTable
	VALUES (@activityId,@commaDelimitedString)

	RETURN

END




