CREATE TABLE [resources].[ResourceVersionAuthor](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[AuthorUserId] [int] NULL,
	[AuthorName] [nvarchar](100) NOT NULL,
	[Organisation] [nvarchar](100) NULL,
	[Role] [nvarchar](100) NULL,
	[IsContributor] [bit] NOT NULL DEFAULT 0,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ResourceVersionAuthor] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceVersionAuthor]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersionAuthor_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionAuthor] CHECK CONSTRAINT [FK_ResourceVersionAuthor_ResourceVersion]
GO

ALTER TABLE [resources].[ResourceVersionAuthor]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersionAuthor_User] FOREIGN KEY([AuthorUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionAuthor] CHECK CONSTRAINT [FK_ResourceVersionAuthor_User]
GO