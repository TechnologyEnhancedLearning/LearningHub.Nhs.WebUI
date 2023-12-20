
CREATE TABLE [resources].[Resource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceTypeId] [int] NOT NULL,
	[CurrentResourceVersionId] [int] NULL,
    [DuplicatedFromResourceVersionId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_Resource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [resources].[Resource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_CurrentResourceVersion] FOREIGN KEY([CurrentResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[Resource] CHECK CONSTRAINT [FK_Resource_CurrentResourceVersion]
GO

ALTER TABLE [resources].[Resource]  WITH CHECK ADD  CONSTRAINT [FK_Resource_ResourceType] FOREIGN KEY([ResourceTypeId])
REFERENCES [resources].[ResourceType] ([Id])
GO

ALTER TABLE [resources].[Resource] CHECK CONSTRAINT [FK_Resource_ResourceType]
GO
