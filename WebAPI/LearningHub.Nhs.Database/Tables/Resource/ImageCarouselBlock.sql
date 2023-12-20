CREATE TABLE [resources].[ImageCarouselBlock]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [BlockId] [int] NOT NULL,
    [ImageBlockCollectionId] [int] NOT NULL,
    [Description] [nvarchar](250) NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_ImageCarouselBlock] PRIMARY KEY CLUSTERED
    (
        [Id] ASC
    ),
)
    GO

ALTER TABLE [resources].[ImageCarouselBlock]  WITH CHECK ADD CONSTRAINT [FK_ImageCarouselBlock_BlockId] FOREIGN KEY([BlockId])
    REFERENCES [resources].[Block] ([Id])
    GO

ALTER TABLE [resources].[ImageCarouselBlock] CHECK CONSTRAINT [FK_ImageCarouselBlock_BlockId]
    GO

ALTER TABLE [resources].[ImageCarouselBlock] WITH CHECK ADD CONSTRAINT [FK_ImageCarouselBlock_ImageBlockCollectionId] FOREIGN KEY([ImageBlockCollectionId])
    REFERENCES [resources].[BlockCollection] ([Id])
    GO

ALTER TABLE [resources].[ImageCarouselBlock] CHECK CONSTRAINT [FK_ImageCarouselBlock_ImageBlockCollectionId]
    GO
