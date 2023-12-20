CREATE TABLE [resources].[QuestionAnswer]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [QuestionBlockId] [int] NOT NULL,
    [Order] [int] NOT NULL,
    [Status] [int] NOT NULL,
    [BlockCollectionId] [int] NULL,
    [ImageAnnotationId] [int] NULL,
    [ImageAnnotationOrder] [int] NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_QuestionAnswer] PRIMARY KEY CLUSTERED
(
[Id] ASC
),
    )
    GO

ALTER TABLE [resources].[QuestionAnswer]  WITH CHECK ADD CONSTRAINT [FK_QuestionAnswer_QuestionBlockId] FOREIGN KEY([QuestionBlockId])
    REFERENCES [resources].[QuestionBlock] ([Id])
    GO

ALTER TABLE [resources].[QuestionAnswer] CHECK CONSTRAINT [FK_QuestionAnswer_QuestionBlockId]
    GO

ALTER TABLE [resources].[QuestionAnswer]  WITH CHECK ADD CONSTRAINT [FK_QuestionAnswer_BlockCollectionId] FOREIGN KEY([BlockCollectionId])
    REFERENCES [resources].[BlockCollection] ([Id])
    GO

ALTER TABLE [resources].[QuestionAnswer] CHECK CONSTRAINT [FK_QuestionAnswer_BlockCollectionId]
    GO

ALTER TABLE [resources].[QuestionAnswer]  WITH CHECK ADD CONSTRAINT [FK_QuestionAnswer_ImageAnnotationId] FOREIGN KEY([ImageAnnotationId])
    REFERENCES [resources].[ImageAnnotation] ([Id])
    GO

ALTER TABLE [resources].[QuestionAnswer] CHECK CONSTRAINT [FK_QuestionAnswer_ImageAnnotationId]
    GO

CREATE NONCLUSTERED INDEX [NCI_QuestionAnswer] ON [resources].[QuestionAnswer] ([Deleted], [QuestionBlockId]);
    GO