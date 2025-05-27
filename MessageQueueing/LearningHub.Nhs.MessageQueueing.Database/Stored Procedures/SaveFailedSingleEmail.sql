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
