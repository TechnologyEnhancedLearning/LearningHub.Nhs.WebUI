CREATE TABLE [hub].[AttributeType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_attributeTypeTBL] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[AttributeType] ADD  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [hub].[AttributeType] ADD  DEFAULT (sysdatetimeoffset()) FOR [AmendDate]
GO