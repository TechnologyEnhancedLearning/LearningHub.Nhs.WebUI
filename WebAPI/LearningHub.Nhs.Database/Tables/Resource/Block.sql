CREATE TABLE [resources].[Block]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BlockCollectionId] [int] NOT NULL,
	[Order] [int] NOT NULL,
	[Title] [nvarchar](200) NULL,
	[BlockType] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_Block] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[Block] WITH CHECK ADD CONSTRAINT [FK_Block_BlockCollectionId] FOREIGN KEY([BlockCollectionId])
REFERENCES [resources].[BlockCollection] ([Id])
GO

ALTER TABLE [resources].[Block] CHECK CONSTRAINT [FK_Block_BlockCollectionId]
GO

CREATE NONCLUSTERED INDEX [NCI_Block] ON [resources].[Block] ([BlockCollectionId], [Deleted]);
GO