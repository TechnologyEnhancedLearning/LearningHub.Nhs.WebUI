CREATE TABLE [resources].[AssessmentResourceVersion]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [ResourceVersionId] [int] NOT NULL,
    [AssessmentType] [int] NOT NULL,
    [AssessmentContentId] [int] NULL,
    [EndGuidanceId] [int] NULL,
    [MaximumAttempts] [int] NULL,
    [PassMark] [int] NULL,
    [AnswerInOrder] [bit] NOT NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_AssessmentResourceVersion] PRIMARY KEY CLUSTERED
(
[Id] ASC
),
    )
    GO

ALTER TABLE [resources].[AssessmentResourceVersion]  WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
    REFERENCES [resources].[ResourceVersion] ([Id])
    GO

ALTER TABLE [resources].[AssessmentResourceVersion] CHECK CONSTRAINT [FK_AssessmentResourceVersion_ResourceVersion]
    GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_AssessmentResourceVersion_ResourceVersionId]
    ON [resources].[AssessmentResourceVersion]([ResourceVersionId] ASC);
GO

ALTER TABLE [resources].[AssessmentResourceVersion] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceVersion_AssessmentContentId] FOREIGN KEY([AssessmentContentId])
    REFERENCES [resources].[BlockCollection] ([Id])
    GO

ALTER TABLE [resources].[AssessmentResourceVersion] CHECK CONSTRAINT [FK_AssessmentResourceVersion_AssessmentContentId]
    GO

ALTER TABLE [resources].[AssessmentResourceVersion] WITH CHECK ADD CONSTRAINT [FK_AssessmentResourceVersion_EndGuidanceId] FOREIGN KEY([EndGuidanceId])
    REFERENCES [resources].[BlockCollection] ([Id])
    GO

ALTER TABLE [resources].[AssessmentResourceVersion] CHECK CONSTRAINT [FK_AssessmentResourceVersion_EndGuidanceId]
    GO
