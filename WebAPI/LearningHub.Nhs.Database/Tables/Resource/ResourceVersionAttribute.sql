CREATE TABLE [resources].[ResourceVersionAttribute](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[AttributeId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ResourceVersionAttribute] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceVersionAttribute]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersionAttribute_Attribute] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionAttribute] CHECK CONSTRAINT [FK_ResourceVersionAttribute_Attribute]
GO