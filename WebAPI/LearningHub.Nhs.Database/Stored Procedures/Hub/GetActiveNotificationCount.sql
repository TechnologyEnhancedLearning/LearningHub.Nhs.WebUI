-------------------------------------------------------------------------------
-- Author       Swapnamol ABraham
-- Created      29 Sep 2025
-- Purpose      Move User notification EF call into the SP
--
-- Modification History
--
-- 29 Sep 2025	SA	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [hub].[GetActiveNotificationCount]
    @UserId INT,
    @UserTimezoneOffset int = NULL
AS
BEGIN
    SET NOCOUNT ON;
	DECLARE @NOW datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

    SELECT 1 as Id, COUNT(*) AS UserNotificationCount
    FROM [hub].[Notification] AS n
    LEFT JOIN [hub].[UserNotification] AS t
        ON n.Id = t.NotificationId 
        AND t.UserId = @UserId
        AND t.Deleted = 0
    WHERE n.Deleted = 0
      AND @Now BETWEEN n.StartDate AND n.EndDate
      AND (n.IsUserSpecific = 0 OR t.UserId = @UserId)
      AND (t.Id IS NULL OR t.Dismissed = 0)
      AND t.ReadOnDate IS NULL;
END
