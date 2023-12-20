CREATE TABLE [hierarchy].[Publication](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NULL,
	[NodeVersionId] [int] NULL,
	[Notes] [nvarchar](4000) NOT NULL,
	[SubmittedToSearch] [bit] NOT NULL DEFAULT 0,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_Publication] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hierarchy].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hierarchy].[Publication] CHECK CONSTRAINT [FK_Publication_CreateUser]
GO

ALTER TABLE [hierarchy].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_NodeVersion] FOREIGN KEY([NodeVersionId])
REFERENCES [hierarchy].[NodeVersion] ([Id])
GO

ALTER TABLE [hierarchy].[Publication] CHECK CONSTRAINT [FK_Publication_NodeVersion]
GO

ALTER TABLE [hierarchy].[Publication]  WITH CHECK ADD  CONSTRAINT [FK_Publication_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [hierarchy].[Publication] CHECK CONSTRAINT [FK_Publication_ResourceVersion]
GO
