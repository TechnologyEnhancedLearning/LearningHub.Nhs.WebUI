CREATE TABLE [dbo].[QueueRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RequestTypeId] [int] NOT NULL,
	[Recipient] [nvarchar](255) NOT NULL,
	[TemplateId] [nvarchar](50) NOT NULL,
	[Personalisation] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[NotificationId] [nvarchar](100) NULL,
	[RetryCount] [int] NULL,
	[CreatedAt] [datetimeoffset](7) NOT NULL,
	[DeliverAfter] [datetimeoffset](7) NULL,
	[SentAt] [datetimeoffset](7) NULL,
	[LastAttemptAt] [datetimeoffset](7) NULL,
	[ErrorMessage] [nvarchar](max) NULL,
 CONSTRAINT [PK_QueueRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[QueueRequests]  WITH CHECK ADD  CONSTRAINT [FK_QueueRequests_RequestStatus] FOREIGN KEY([Status])
REFERENCES [dbo].[RequestStatus] ([Id])
GO

ALTER TABLE [dbo].[QueueRequests] CHECK CONSTRAINT [FK_QueueRequests_RequestStatus]
GO

ALTER TABLE [dbo].[QueueRequests]  WITH CHECK ADD  CONSTRAINT [FK_QueueRequests_RequestType] FOREIGN KEY([RequestTypeId])
REFERENCES [dbo].[RequestType] ([Id])
GO

ALTER TABLE [dbo].[QueueRequests] CHECK CONSTRAINT [FK_QueueRequests_RequestType]
GO