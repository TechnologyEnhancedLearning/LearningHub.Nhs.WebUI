CREATE TABLE [activity].[AssessmentResourceActivityInteraction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssessmentResourceActivityId] [int] NOT NULL,
	[QuestionBlockId] [int] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_AssessmentResourceActivityInteraction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteraction]  WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityInteraction_QuestionBlockId] FOREIGN KEY([QuestionBlockId])
REFERENCES [resources].[QuestionBlock] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteraction] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteraction_QuestionBlockId]
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteraction]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentResourceActivityInteraction_AmendUser] FOREIGN KEY([AmendUserID])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteraction] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteraction_AmendUser]
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteraction]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentResourceActivityInteraction_CreateUser] FOREIGN KEY([CreateUserID])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteraction] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteraction_CreateUser]
GO
ALTER TABLE [activity].[AssessmentResourceActivityInteraction]  WITH CHECK ADD  CONSTRAINT [FK_AssessmentResourceActivityInteraction_AssessmentResourceActivity] FOREIGN KEY([AssessmentResourceActivityId])
REFERENCES [activity].[AssessmentResourceActivity] ([Id])
GO

ALTER TABLE [activity].[AssessmentResourceActivityInteraction] CHECK CONSTRAINT [FK_AssessmentResourceActivityInteraction_AssessmentResourceActivity]
GO

CREATE INDEX IX_AssessmentResourceActivityInteraction_AssessmentResourceActivity ON [activity].[AssessmentResourceActivityInteraction](AssessmentResourceActivityId)
WITH (FILLFACTOR = 95);
GO