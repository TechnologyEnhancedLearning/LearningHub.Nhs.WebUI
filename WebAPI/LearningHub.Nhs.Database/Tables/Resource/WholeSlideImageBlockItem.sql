CREATE TABLE [resources].[WholeSlideImageBlockItem]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
    [WholeSlideImageBlockId] [int] NOT NULL,
    [WholeSlideImageId] [int] NULL,
    [PlaceholderText] [nvarchar](255) NULL,
    [Order] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_WholeSlideImageBlockItem] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[WholeSlideImageBlockItem] WITH CHECK ADD CONSTRAINT [FK_WholeSlideImageBlockItem_WholeSlideImageBlockId] FOREIGN KEY([WholeSlideImageBlockId])
REFERENCES [resources].[WholeSlideImageBlock] ([Id])
GO

ALTER TABLE [resources].[WholeSlideImageBlockItem] CHECK CONSTRAINT [FK_WholeSlideImageBlockItem_WholeSlideImageBlockId]
GO

ALTER TABLE [resources].[WholeSlideImageBlockItem] WITH CHECK ADD CONSTRAINT [FK_WholeSlideImageBlockItem_WholeSlideImageId] FOREIGN KEY([WholeSlideImageId])
REFERENCES [resources].[WholeSlideImage] ([Id])
GO

ALTER TABLE [resources].[WholeSlideImageBlockItem] CHECK CONSTRAINT [FK_WholeSlideImageBlockItem_WholeSlideImageId]
GO