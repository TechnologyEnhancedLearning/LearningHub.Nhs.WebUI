CREATE TABLE [resources].[ResourceVersionFlag](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[Details] [nvarchar](1024) NULL,
	[IsActive] [bit] NOT NULL,
	[Resolution] [nvarchar](1024) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_ResourceVersion_ResourceVersionFlag] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceVersionFlag]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersionFlag_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionFlag] CHECK CONSTRAINT [FK_ResourceVersionFlag_ResourceVersion]
GO

ALTER TABLE [resources].[ResourceVersionFlag]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersionFlag_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO