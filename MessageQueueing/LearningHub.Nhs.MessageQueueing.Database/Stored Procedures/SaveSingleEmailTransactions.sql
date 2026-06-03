-------------------------------------------------------------------------------
-- Author       Arunima George
-- Created      27-05-2025
-- Purpose      Save all one-off(like otp) email request transactions.
--
-- Modification History
--
-- 27-05-2025  Arunima George	Initial Revision
-- 03-06-2026  Arunima George   TD-7354
-------------------------------------------------------------------------------

CREATE PROCEDURE [dbo].[SaveSingleEmailTransactions]
	@Recipient nvarchar(255),
	@TemplateId nvarchar(50),
	@Personalisation nvarchar(max) = NULL,
	@Status int,
	@ErrorMessage nvarchar(max) = NULL,
	@UserTimezoneOffset int = NULL
AS
BEGIN	
			DECLARE @CreateDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

			insert into [dbo].[QueueRequests] (RequestTypeId, Recipient, TemplateId, Personalisation, Status, CreatedAt, LastAttemptAt,ErrorMessage )
			values (3, @Recipient, @TemplateId, @Personalisation, @Status, @CreateDate, @CreateDate, @ErrorMessage);
END
GO
