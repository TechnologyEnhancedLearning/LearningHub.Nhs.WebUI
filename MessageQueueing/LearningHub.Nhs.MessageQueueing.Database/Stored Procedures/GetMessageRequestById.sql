-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      18-03-2026
-- Purpose      To get message request details by id.
--
-- Modification History
--
-- 18-03-2026  Arunima George	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[GetMessageRequestById]
@id int
AS
BEGIN
SET NOCOUNT ON;
				select qr.Id,rt.RequestType,Recipient,rs.RequestStatus,RetryCount,CreatedAt,DeliverAfter,SentAt,LastAttemptAt,ErrorMessage
				from dbo.QueueRequests qr
				left join RequestType as rt on rt.Id = qr.RequestTypeId
				left join RequestStatus as rs on rs.Id = qr.Status
				where qr.Id = @Id

		
END
GO
