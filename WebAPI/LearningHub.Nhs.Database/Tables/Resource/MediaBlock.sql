CREATE TABLE [resources].[MediaBlock]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [BlockId] [int] NOT NULL,
    [MediaType] [int] NOT NULL,
    [AttachmentId] [int] NULL,
    [ImageId] [int] NULL,
    [VideoId] [int] NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_MediaBlock] PRIMARY KEY CLUSTERED
        ([Id] ASC),
)
GO

ALTER TABLE [resources].[MediaBlock] WITH CHECK ADD CONSTRAINT [FK_MediaBlock_BlockId] FOREIGN KEY([BlockId])
    REFERENCES [resources].[Block] ([Id])
    GO

ALTER TABLE [resources].[MediaBlock] CHECK CONSTRAINT [FK_MediaBlock_BlockId]
    GO

ALTER TABLE [resources].[MediaBlock] WITH CHECK ADD CONSTRAINT [FK_MediaBlock_AttachmentId] FOREIGN KEY([AttachmentId])
    REFERENCES [resources].[Attachment] ([Id])
    GO

ALTER TABLE [resources].[MediaBlock] CHECK CONSTRAINT [FK_MediaBlock_AttachmentId]
    GO

ALTER TABLE [resources].[MediaBlock] WITH CHECK ADD CONSTRAINT [FK_MediaBlock_ImageId] FOREIGN KEY([ImageId])
    REFERENCES [resources].[Image] ([Id])
    GO

ALTER TABLE [resources].[MediaBlock] CHECK CONSTRAINT [FK_MediaBlock_ImageId]
    GO

ALTER TABLE [resources].[MediaBlock] WITH CHECK ADD CONSTRAINT [FK_MediaBlock_VideoId] FOREIGN KEY([VideoId])
    REFERENCES [resources].[Video] ([Id])
    GO

ALTER TABLE [resources].[MediaBlock] CHECK CONSTRAINT [FK_MediaBlock_VideoId]
    GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_MediaBlock_BlockId] 
    ON [resources].[MediaBlock]([BlockId] ASC);
GO 
