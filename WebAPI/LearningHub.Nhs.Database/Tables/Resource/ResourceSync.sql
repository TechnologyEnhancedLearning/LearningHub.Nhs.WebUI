CREATE TABLE [resources].[ResourceSync](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceId] [int] NOT NULL,
	[UserId][int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_ResourceSync] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceSync]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSync_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ResourceSync]  WITH CHECK ADD  CONSTRAINT [FK_ResourceSync_ResourceVersion] FOREIGN KEY([ResourceId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO