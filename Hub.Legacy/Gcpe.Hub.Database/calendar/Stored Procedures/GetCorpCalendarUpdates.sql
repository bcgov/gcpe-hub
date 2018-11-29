




CREATE PROCEDURE [calendar].[GetCorpCalendarUpdates]
    @UserID int = 0,
    @IsInAppOwnerList bit=0
AS
BEGIN
    SET NOCOUNT ON;

BEGIN TRY

    DECLARE @UserMinistryID TABLE
    ( MinistryId UNIQUEIDENTIFIER )
    INSERT INTO @UserMinistryID
    SELECT MinistryId FROM SystemUserMinistry WHERE SystemUserId=@UserID AND IsActive=1
    
    DECLARE @AccessibleSharedWithActivities TABLE
    ( ActivityId int)
    INSERT INTO @AccessibleSharedWithActivities
    SELECT ActivityId 
    FROM ActivitySharedWith
    WHERE MinistryId IN (SELECT MinistryId FROM @UserMinistryID)

    IF @IsInAppOwnerList = 1
      BEGIN 
         SELECT TOP 5 nf.*
         FROM NewsFeed nf
              INNER JOIN Activity a on nf.ActivityId = a.Id
         ORDER BY nf.CreatedDateTime DESC
      END
    ELSE
      BEGIN
        SELECT TOP 5 nf.*
         FROM NewsFeed nf
              INNER JOIN Activity a on nf.ActivityId = a.Id
         WHERE (a.ContactMinistryId in (Select MinistryId FROM @UserMinistryID) 
                     OR a.Id in (Select ActivityId FROM @AccessibleSharedWithActivities))
         ORDER BY nf.CreatedDateTime DESC
      END

END TRY


BEGIN CATCH

    -- Raise an error with the details of the exception   
      DECLARE @ErrMsg nvarchar(4000),
              @ErrSeverity int 

      SELECT @ErrMsg = ERROR_MESSAGE(), 
             @ErrSeverity = ERROR_SEVERITY() 

      RAISERROR(@ErrMsg, @ErrSeverity, 1) 


END CATCH

END




