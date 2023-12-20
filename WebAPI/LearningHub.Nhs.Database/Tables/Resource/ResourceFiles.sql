CREATE TABLE [dbo].[ResourceFiles](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceId] [int] NOT NULL,
	[Filename] [nvarchar](255) NOT NULL,
	[Location] [nvarchar](512) NOT NULL,
	[FileType] [int] NOT NULL,
	[FileSize] [bigint] NULL,
 CONSTRAINT [PK_ResourceFiles] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[ResourceFiles]  WITH CHECK ADD  CONSTRAINT [FK_ResourceFiles_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id])
GO

ALTER TABLE [dbo].[ResourceFiles] CHECK CONSTRAINT [FK_ResourceFiles_Resource]
GO