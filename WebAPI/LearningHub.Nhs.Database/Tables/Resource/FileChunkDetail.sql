CREATE TABLE [resources].[FileChunkDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NULL,
	[FileName] [nvarchar](255) NOT NULL,
	[ChunkCount] [int] NOT NULL,
	[FilePath] [nvarchar](1024) NOT NULL,
	[FileSizeKb] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_FileChunkDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[FileChunkDetail]  WITH CHECK ADD  CONSTRAINT [FK_FileChunkDetail_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[FileChunkDetail] CHECK CONSTRAINT [FK_FileChunkDetail_ResourceVersion]
GO
