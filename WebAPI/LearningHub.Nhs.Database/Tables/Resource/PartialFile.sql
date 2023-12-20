CREATE TABLE [resources].[PartialFile]
(
	[Id] [int] IDENTITY(1,1) NOT NULL, 
    [FileId] [int] NOT NULL, 
    [TotalSize] [bigint] NOT NULL, 
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_PartialFile] PRIMARY KEY CLUSTERED 
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[PartialFile] WITH CHECK ADD CONSTRAINT [FK_PartialFile_FileId] FOREIGN KEY([FileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[PartialFile] CHECK CONSTRAINT [FK_PartialFile_FileId]
GO

ALTER TABLE [resources].[PartialFile] ADD CONSTRAINT [UC_PartialFile_FileId] UNIQUE ([FileId])
GO
