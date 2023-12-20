CREATE TABLE [hierarchy].[PublicationLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PublicationId] [int] NOT NULL,
	[NodeId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL
) ON [PRIMARY]
GO

ALTER TABLE [hierarchy].[PublicationLog]  WITH CHECK ADD  CONSTRAINT [FK_PublicationLog_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hierarchy].[PublicationLog]  WITH CHECK ADD  CONSTRAINT [FK_PublicationLog_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[PublicationLog]  WITH CHECK ADD  CONSTRAINT [FK_PublicationLog_Publication] FOREIGN KEY([PublicationId])
REFERENCES [hierarchy].[Publication] ([Id])
GO