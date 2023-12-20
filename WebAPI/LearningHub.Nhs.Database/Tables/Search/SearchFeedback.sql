CREATE TABLE [analytics].[SearchFeedback](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SearchId] [int] NOT NULL,
	[SearchFeedback] [nvarchar](1000) NOT NULL,
	[CurrentUrl] [nvarchar](1000) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_SearchFeedback] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [analytics].[SearchFeedback] WITH CHECK ADD CONSTRAINT [FK_SearchFeedback_SearchFeedback_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[SearchFeedback] CHECK CONSTRAINT [FK_SearchFeedback_SearchFeedback_AmendUser]
GO

ALTER TABLE [analytics].[SearchFeedback]  WITH CHECK ADD  CONSTRAINT [FK_SearchFeedback_SearchFeedback_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[SearchFeedback] CHECK CONSTRAINT [FK_SearchFeedback_SearchFeedback_CreateUser]
GO

ALTER TABLE [analytics].[SearchFeedback]  WITH CHECK ADD  CONSTRAINT [FK_SearchFeedback_SearchFeedback_SearchId] FOREIGN KEY([SearchId])
REFERENCES [analytics].[SearchTerm] ([Id])
GO

ALTER TABLE [analytics].[SearchFeedback] CHECK CONSTRAINT [FK_SearchFeedback_SearchFeedback_SearchId]
GO