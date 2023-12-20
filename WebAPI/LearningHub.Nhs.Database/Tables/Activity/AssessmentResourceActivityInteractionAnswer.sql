CREATE TABLE [activity].[AssessmentResourceActivityInteractionAnswer](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssessmentResourceActivityInteractionId] [int] NOT NULL,
	[QuestionAnswerId] [int] NOT NULL,
    [MatchedQuestionAnswerId] [int] NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_AssessmentResourceActivityInteractionAnswer] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_AmendUser] FOREIGN KEY([AmendUserID])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_AmendUser]
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_QuestionAnswerId] FOREIGN KEY([QuestionAnswerId])
REFERENCES [resources].[QuestionAnswer] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_QuestionAnswerId]
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_MatchedQuestionAnswerId] FOREIGN KEY([MatchedQuestionAnswerId])
REFERENCES [resources].[QuestionAnswer] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_MatchedQuestionAnswerId]
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_CreateUser] FOREIGN KEY([CreateUserID])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_CreateUser]
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_AssessmentResourceActivityInteraction] FOREIGN KEY([AssessmentResourceActivityInteractionId])
REFERENCES [activity].[AssessmentResourceActivityInteraction] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteractionAnswer] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteractionAnswer_AssessmentResourceActivityInteraction]
GO

CREATE INDEX IX_AssessmentResourceActivityInteraction_AssessmentResourceActivity ON [activity].[AssessmentResourceActivityInteractionAnswer]([AssessmentResourceActivityInteractionId])
WITH (FILLFACTOR = 95);
GO