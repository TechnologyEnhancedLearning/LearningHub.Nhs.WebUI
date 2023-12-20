CREATE TABLE [resources].[WholeSlideImageFile]
(
	[Id] [int] IDENTITY(1,1) NOT NULL, 
    [FileId] [int] NOT NULL, 
    [Status] [int] NOT NULL, 
    [ProcessingErrorMessage] [nvarchar](1024) NULL,
    [Width] [int] NULL,
    [Height] [int] NULL,
    [DeepZoomTileSize] [int] NULL,
    [DeepZoomOverlap] [int] NULL,
    [Layers] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_WholeSlideImageFile] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[WholeSlideImageFile] WITH CHECK ADD CONSTRAINT [FK_WholeSlideImageFile_FileId] FOREIGN KEY([FileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[WholeSlideImageFile] CHECK CONSTRAINT [FK_WholeSlideImageFile_FileId]
GO

ALTER TABLE [resources].[WholeSlideImageFile] ADD CONSTRAINT [UC_WholeSlideImageFile_FileId] UNIQUE ([FileId])
GO
