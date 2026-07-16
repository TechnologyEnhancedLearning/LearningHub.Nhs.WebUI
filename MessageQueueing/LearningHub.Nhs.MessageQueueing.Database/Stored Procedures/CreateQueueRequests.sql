-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Create email requests.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-- 03-06-2026  Arunima George   TD-7354
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[CreateQueueRequests]
    @QueueRequests dbo.QueueRequestTableType READONLY,
	@UserTimezoneOffset int = NULL
AS
BEGIN
	DECLARE @CreateDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

    INSERT INTO QueueRequests (RequestTypeId, Recipient, TemplateId, Personalisation, Status, RetryCount, CreatedAt, DeliverAfter)
    SELECT 1, Recipient, TemplateId, Personalisation, 1, 0, @CreateDate, SWITCHOFFSET(DeliverAfter, '+00:00')
    FROM @QueueRequests;
END
