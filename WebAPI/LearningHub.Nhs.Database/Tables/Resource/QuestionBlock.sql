CREATE TABLE [resources].[QuestionBlock]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [BlockId] [int] NOT NULL,
    [QuestionBlockCollectionId] [int] NOT NULL,
    [FeedbackBlockCollectionId] [int] NOT NULL,
    [QuestionType] [int] NOT NULL,
    [AllowReveal] [bit] NULL,
    [Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_QuestionBlock] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[QuestionBlock]  WITH CHECK ADD CONSTRAINT [FK_QuestionBlock_BlockId] FOREIGN KEY([BlockId])
REFERENCES [resources].[Block] ([Id])
GO

ALTER TABLE [resources].[QuestionBlock] CHECK CONSTRAINT [FK_QuestionBlock_BlockId]
GO

ALTER TABLE [resources].[QuestionBlock] WITH CHECK ADD CONSTRAINT [FK_QuestionBlock_QuestionBlockCollectionId] FOREIGN KEY([QuestionBlockCollectionId])
REFERENCES [resources].[BlockCollection] ([Id])
GO

ALTER TABLE [resources].[QuestionBlock] CHECK CONSTRAINT [FK_QuestionBlock_QuestionBlockCollectionId]
GO

ALTER TABLE [resources].[QuestionBlock] WITH CHECK ADD CONSTRAINT [FK_QuestionBlock_FeedbackBlockCollectionId] FOREIGN KEY([FeedbackBlockCollectionId])
REFERENCES [resources].[BlockCollection] ([Id])
GO

ALTER TABLE [resources].[QuestionBlock] CHECK CONSTRAINT [FK_QuestionBlock_FeedbackBlockCollectionId]
GO
