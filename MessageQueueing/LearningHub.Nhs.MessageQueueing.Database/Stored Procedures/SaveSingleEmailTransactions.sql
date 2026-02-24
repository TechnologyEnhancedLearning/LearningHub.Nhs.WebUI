-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Save all one-off(like otp) email request transactions.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[SaveSingleEmailTransactions]
	@Recipient nvarchar(255),
	@TemplateId nvarchar(50),
	@Personalisation nvarchar(max) = NULL,
	@Status int,
	@ErrorMessage nvarchar(max) = NULL
AS
BEGIN	
			insert into [dbo].[QueueRequests] (RequestTypeId, Recipient, TemplateId, Personalisation, Status, CreatedAt, LastAttemptAt,ErrorMessage )
			values (3, @Recipient, @TemplateId, @Personalisation, @Status, SYSDATETIMEOFFSET(), SYSDATETIMEOFFSET(), @ErrorMessage);
END
GO
