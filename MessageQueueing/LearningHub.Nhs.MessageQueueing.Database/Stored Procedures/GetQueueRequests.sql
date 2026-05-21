-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Fetch pending/failed email requests from QueueRequests table.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[GetQueueRequests]
AS
BEGIN

				select Id,Recipient,TemplateId,Personalisation,Status,RetryCount
				from dbo.QueueRequests
where RequestTypeId = 1 and Status in (1,3) and RetryCount < 3 and (DeliverAfter is null or DeliverAfter <= SYSDATETIMEOFFSET())
	
END
GO