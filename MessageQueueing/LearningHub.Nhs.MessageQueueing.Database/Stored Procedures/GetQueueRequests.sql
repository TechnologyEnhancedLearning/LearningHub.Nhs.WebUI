CREATE PROCEDURE [dbo].[GetQueueRequests]
AS
BEGIN

				select Id,Recipient,TemplateId,Personalisation,Status,RetryCount
				from dbo.QueueRequests
where RequestTypeId = 1 and Status in (1,3) and RetryCount < 3 and (DeliverAfter is null or DeliverAfter <= SYSDATETIMEOFFSET())
	
END
GO