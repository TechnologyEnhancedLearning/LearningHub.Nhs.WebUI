CREATE TABLE [resources].[ResourceReferenceDisplayVersion] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceReferenceId] [int] NOT NULL,
	[DisplayName] [nvarchar](255) NULL,
	[VersionStatusId] [int] NOT NULL,
	[PublicationId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_ResourceReferenceDisplayVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceReferenceDisplayVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceReferenceDisplayVersion_ResourceReference] FOREIGN KEY([ResourceReferenceId])
REFERENCES [resources].[ResourceReference] ([Id])
GO

ALTER TABLE [resources].[ResourceReferenceDisplayVersion] CHECK CONSTRAINT [FK_ResourceReferenceDisplayVersion_ResourceReference]
GO

ALTER TABLE [resources].[ResourceReferenceDisplayVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceReferenceDisplayVersion_VersionStatus] FOREIGN KEY([VersionStatusId])
REFERENCES [resources].[VersionStatus] ([Id])
GO

ALTER TABLE [resources].[ResourceReferenceDisplayVersion] CHECK CONSTRAINT [FK_ResourceReferenceDisplayVersion_VersionStatus]
GO
