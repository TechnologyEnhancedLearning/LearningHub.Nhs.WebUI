-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Save one-off(like otp emails) failed email request.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[SaveFailedSingleEmail]
	@Recipient nvarchar(255),
	@TemplateId nvarchar(50),
	@Personalisation nvarchar(max),
	@ErrorMessage nvarchar(max)
AS
BEGIN	
			insert into [dbo].[QueueRequests] (RequestTypeId, Recipient, TemplateId, Personalisation, Status, CreatedAt, LastAttemptAt,ErrorMessage )
			values (3, @Recipient, @TemplateId, @Personalisation, 3, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), @ErrorMessage);
END
GO
