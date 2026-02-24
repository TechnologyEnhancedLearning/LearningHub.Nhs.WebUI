CREATE TYPE [dbo].[QueueRequestTableType] AS TABLE(
	[Recipient] [nvarchar](255) NOT NULL,
	[TemplateId] [nvarchar](50) NOT NULL,
	[Personalisation] [nvarchar](max) NULL,
	[DeliverAfter] [datetimeoffset](7) NULL
)
GO
