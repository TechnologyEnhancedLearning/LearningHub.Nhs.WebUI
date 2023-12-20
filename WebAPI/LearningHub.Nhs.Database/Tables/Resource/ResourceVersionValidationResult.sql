CREATE TABLE [resources].[ResourceVersionValidationResult](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[Success] [bit] NOT NULL,
	[Details] [nvarchar](1024) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ResourceVersionValidationResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceVersionValidationResult] ADD  CONSTRAINT [FK_ResourceVersionValidationResult_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionValidationResult] ADD  CONSTRAINT [FK_ResourceVersionValidationResult_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionValidationResult] ADD  CONSTRAINT [FK_ResourceVersionValidationResult_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO