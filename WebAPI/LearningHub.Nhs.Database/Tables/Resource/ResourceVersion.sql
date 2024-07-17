CREATE TABLE [resources].[ResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceId] [int] NOT NULL,
	[VersionStatusId] [int] NOT NULL,
	[ResourceAccessibilityId] [int] NOT NULL DEFAULT 4,
	[PublicationId] [int] NULL,
	[MajorVersion] [int] NULL,
	[MinorVersion] [int] NULL,
	[Title] [nvarchar](255) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[AdditionalInformation] [nvarchar](250) NOT NULL,
	[ReviewDate] [datetimeoffset](7) NULL,
	[HasCost] [bit] NOT NULL,
	[Cost] [decimal](8, 2) NULL,
	[ResourceLicenceId] [int] NULL,
	[SensitiveContent] [bit] NOT NULL DEFAULT 0,
	[CertificateEnabled] [bit] NULL,
	[ProviderId] [int] NULL,
	[PrimaryCatalogueNodeId] [int] NOT NULL DEFAULT 1,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersion_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersion_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id])
GO

ALTER TABLE [resources].[ResourceVersion] CHECK CONSTRAINT [FK_ResourceVersion_Resource]
GO

ALTER TABLE [resources].[ResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersion_VersionStatus] FOREIGN KEY([VersionStatusId])
REFERENCES [resources].[VersionStatus] ([Id])
GO

ALTER TABLE [resources].[ResourceVersion] CHECK CONSTRAINT [FK_ResourceVersion_VersionStatus]
GO

ALTER TABLE [resources].[ResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersion_ResourceLicence] FOREIGN KEY([ResourceLicenceId])
REFERENCES [resources].[ResourceLicence] ([Id])
GO

ALTER TABLE [resources].[ResourceVersion] CHECK CONSTRAINT [FK_ResourceVersion_ResourceLicence]
GO

ALTER TABLE [resources].[ResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersion_Provider] FOREIGN KEY([ProviderId])
REFERENCES [hub].[Provider] ([Id])
GO

ALTER TABLE [resources].[ResourceVersion] CHECK CONSTRAINT [FK_ResourceVersion_Provider]
GO

ALTER TABLE [resources].[ResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersion_PrimaryCatalogueNode] FOREIGN KEY([PrimaryCatalogueNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [resources].[ResourceVersion] CHECK CONSTRAINT [FK_ResourceVersion_PrimaryCatalogueNode]
GO

CREATE NONCLUSTERED INDEX [IX_ResourceVersion_VersionStatusId]
	ON [resources].[ResourceVersion]([Deleted] ASC, [VersionStatusId] ASC) INCLUDE([Id], [ResourceId], [PublicationId], [Title], [CreateDate], [AmendUserId], [AmendDate]);
GO
