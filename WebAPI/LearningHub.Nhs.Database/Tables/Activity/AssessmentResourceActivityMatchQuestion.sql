CREATE TABLE [activity].[AssessmentResourceActivityMatchQuestion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AssessmentResourceActivityId] [int] NOT NULL,
    [QuestionNumber] [int] NOT NULL,
	[Order] [int] NOT NULL,
    [FirstMatchAnswerId] [int] NOT NULL,
    [SecondMatchAnswerId] [int] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
    CONSTRAINT [PK_AssessmentResourceActivityMatchQuestion] PRIMARY KEY CLUSTERED 
    (
        [Id] ASC
    )
    ) ON [PRIMARY]
GO

ALTER TABLE [activity].[AssessmentResourceActivityMatchQuestion] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityMatchQuestion_FirstMatchAnswerId] FOREIGN KEY ([FirstMatchAnswerId])
    REFERENCES [resources].[QuestionAnswer] ([Id])
    GO

ALTER TABLE [activity].[AssessmentResourceActivityMatchQuestion] CHECK CONSTRAINT [FK_AssessmentResourceActivityMatchQuestion_FirstMatchAnswerId]
    GO

ALTER TABLE [activity].[AssessmentResourceActivityMatchQuestion] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityMatchQuestion_SecondMatchAnswerId] FOREIGN KEY ([SecondMatchAnswerId])
    REFERENCES [resources].[QuestionAnswer] ([Id])
    GO

ALTER TABLE [activity].[AssessmentResourceActivityMatchQuestion] CHECK CONSTRAINT [FK_AssessmentResourceActivityMatchQuestion_SecondMatchAnswerId]
    GO

ALTER TABLE [activity].[AssessmentResourceActivityMatchQuestion] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceActivityMatchQuestion_AssessmentResourceActivityId] FOREIGN KEY([AssessmentResourceActivityId])
    REFERENCES [activity].[AssessmentResourceActivity] ([Id])
    GO

ALTER TABLE [activity].[AssessmentResourceActivityMatchQuestion] CHECK CONSTRAINT [FK_AssessmentResourceActivityMatchQuestion_AssessmentResourceActivityId]
    GO