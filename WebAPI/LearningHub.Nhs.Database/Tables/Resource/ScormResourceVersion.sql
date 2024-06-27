CREATE TABLE [resources].[ScormResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[FileId] [int] NULL,
	[ContentFilePath] [nvarchar](1024) NULL,
	[DevelopmentId] [nvarchar](30) NULL,
	[EsrLinkTypeId] int NULL DEFAULT 1, 
    [CanDownload] BIT NOT NULL DEFAULT 0,
	[ClearSuspendData] BIT NOT NULL DEFAULT 0,
	[PopupWidth] [int] NULL,
	[PopupHeight] [int] NULL, 
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_ResourceVersion_ScormResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ScormResourceVersion]  ADD  CONSTRAINT [FK_ScormResourceVersion_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ScormResourceVersion]  ADD  CONSTRAINT [FK_ScormResourceVersion_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ScormResourceVersion]  ADD CONSTRAINT [FK_ScormResourceVersion_File] FOREIGN KEY([FileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[ScormResourceVersion]  ADD  CONSTRAINT [FK_ScormResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ScormResourceVersion]  ADD CONSTRAINT [FK_ScormResourceVersion_EsrLinkType] FOREIGN KEY([EsrLinkTypeId])
REFERENCES [resources].[EsrLinkType] ([Id])
GO
CREATE NONCLUSTERED INDEX [IX_Resources_ScormResourceVersion_ResourceVersionId] ON [resources].[ScormResourceVersion]([ResourceVersionId])
GO