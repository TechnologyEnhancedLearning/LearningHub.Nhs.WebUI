CREATE TABLE [resources].[ImageAnnotationMark]
(
    [Id] [int] IDENTITY (1,1) NOT NULL,
    [ImageAnnotationId] [int] NULL,
    [Type] [int] NOT NULL,
    [MarkLabel] [nvarchar](255) NULL,
    [MarkShapeData] [nvarchar](MAX) NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_ImageAnnotationMark] PRIMARY KEY CLUSTERED
    (
        [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[ImageAnnotationMark] WITH CHECK ADD CONSTRAINT  [FK_ImageAnnotationMark_ImageAnnotationId] FOREIGN KEY([ImageAnnotationId])
REFERENCES [resources].[ImageAnnotation] ([Id])
GO

ALTER TABLE [resources].[ImageAnnotationMark] CHECK CONSTRAINT [FK_ImageAnnotationMark_ImageAnnotationId]
GO
