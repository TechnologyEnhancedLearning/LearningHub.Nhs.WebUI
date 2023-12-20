CREATE TABLE [hub].[Attribute](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[AttributeTypeId] [int] NOT NULL DEFAULT(1),
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](400) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Attribute] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[Attribute] ADD  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [hub].[Attribute] ADD  DEFAULT (sysdatetimeoffset()) FOR [AmendDate]
GO

ALTER TABLE [hub].[Attribute]  WITH CHECK ADD  CONSTRAINT [FK_Attribute_AttributeType] FOREIGN KEY([AttributeTypeId])
REFERENCES [hub].[AttributeType] ([Id])
GO

ALTER TABLE [hub].[Attribute] CHECK CONSTRAINT [FK_Attribute_AttributeType]
GO