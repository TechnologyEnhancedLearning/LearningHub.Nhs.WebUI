CREATE TABLE [resources].[CaseResourceVersion]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[BlockCollectionId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_CaseResourceVersion] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[CaseResourceVersion]  WITH CHECK ADD CONSTRAINT [FK_CaseResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[CaseResourceVersion] CHECK CONSTRAINT [FK_CaseResourceVersion_ResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_CaseResourceVersion_ResourceVersionId]
    ON [resources].[CaseResourceVersion]([ResourceVersionId] ASC);
GO

ALTER TABLE [resources].[CaseResourceVersion] WITH CHECK ADD CONSTRAINT [FK_CaseResourceVersion_BlockCollectionId] FOREIGN KEY([BlockCollectionId])
REFERENCES [resources].[BlockCollection] ([Id])
GO

ALTER TABLE [resources].[CaseResourceVersion] CHECK CONSTRAINT [FK_CaseResourceVersion_BlockCollectionId]
GO
