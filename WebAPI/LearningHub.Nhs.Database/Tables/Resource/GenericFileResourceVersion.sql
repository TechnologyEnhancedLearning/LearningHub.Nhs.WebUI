CREATE TABLE [resources].[GenericFileResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[FileId] [int] NOT NULL,
    [ScormAiccContent]    BIT                DEFAULT ((0)) NOT NULL,
	[AuthoredYear] [int] NULL,
	[AuthoredMonth] [int] NULL,
	[AuthoredDayOfMonth] [int] NULL,
	[EsrLinkTypeId] [int] NULL DEFAULT 1, 
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_GenericFileResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[GenericFileResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_GenericFileResource_File] FOREIGN KEY([FileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[GenericFileResourceVersion] CHECK CONSTRAINT [FK_GenericFileResource_File]
GO

ALTER TABLE [resources].[GenericFileResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_GenericFileResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[GenericFileResourceVersion] CHECK CONSTRAINT [FK_GenericFileResourceVersion_ResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_GenericFileResourceVersion]
    ON [resources].[GenericFileResourceVersion]([ResourceVersionId] ASC);
GO
