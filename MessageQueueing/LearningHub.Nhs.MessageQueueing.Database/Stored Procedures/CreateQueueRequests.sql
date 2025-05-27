Create PROCEDURE [dbo].[CreateQueueRequests]
    @QueueRequests dbo.QueueRequestTableType READONLY
AS
BEGIN

    INSERT INTO QueueRequests (RequestTypeId, Recipient, TemplateId, Personalisation, Status, RetryCount, CreatedAt, DeliverAfter)
    SELECT 1, Recipient, TemplateId, Personalisation, 1, 0, SYSDATETIMEOFFSET(), DeliverAfter
    FROM @QueueRequests;
END
GO
