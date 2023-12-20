CREATE TABLE [resources].[ImageAnnotation]
(
    [Id] [int] IDENTITY (1,1) NOT NULL,
    [WholeSlideImageId] [int] NULL,
    [ImageId] [int]  NULL,
    [Order] [int] NOT NULL,
    [Label] [nvarchar](255) NULL,
    [Description] [nvarchar](1000) NULL,
    [PinXCoordinate] decimal(22,19) NULL,
    [PinYCoordinate] decimal(22,19) NULL,
    [Colour] [int] NOT NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT  [PK_Resources_ImageAnnotation] PRIMARY KEY CLUSTERED
    (
        [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[ImageAnnotation] WITH CHECK ADD CONSTRAINT  [FK_ImageAnnotation_WholeSlideImageId] FOREIGN KEY([WholeSlideImageId])
REFERENCES [resources].[WholeSlideImage] ([Id])
GO

ALTER TABLE [resources].[ImageAnnotation] CHECK CONSTRAINT [FK_ImageAnnotation_WholeSlideImageId]
GO

ALTER TABLE [resources].[ImageAnnotation] WITH CHECK ADD CONSTRAINT [FK_ImageAnnotation_ImageId] FOREIGN KEY([ImageId]) 
REFERENCES [resources].[Image] ([Id])
GO

ALTER TABLE [resources].[ImageAnnotation] CHECK CONSTRAINT [FK_ImageAnnotation_ImageId]
GO