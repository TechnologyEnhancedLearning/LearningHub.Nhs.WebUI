CREATE TABLE [hierarchy].[NodePathDisplayVersion] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodePathId] [int] NOT NULL,
	[DisplayName] [nvarchar](255) NULL,
	[VersionStatusId] [int] NOT NULL,
	[PublicationId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodePathVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hierarchy].[NodePathDisplayVersion]  WITH CHECK ADD  CONSTRAINT [FK_NodePathDisplayVersion_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO

ALTER TABLE [hierarchy].[NodePathDisplayVersion] CHECK CONSTRAINT [FK_NodePathDisplayVersion_NodePath]
GO

ALTER TABLE [hierarchy].[NodePathDisplayVersion]  WITH CHECK ADD  CONSTRAINT [FK_NodePathDisplayVersion_VersionStatus] FOREIGN KEY([VersionStatusId])
REFERENCES [resources].[VersionStatus] ([Id])
GO

ALTER TABLE [hierarchy].[NodePathDisplayVersion] CHECK CONSTRAINT [FK_NodePathDisplayVersion_VersionStatus]
GO
