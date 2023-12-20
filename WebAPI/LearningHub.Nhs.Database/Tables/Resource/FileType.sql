CREATE TABLE [resources].[FileType](
	[Id] [int] NOT NULL,
	[DefaultResourceTypeId] [int] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](512) NOT NULL,
	[Extension] [nvarchar](10) NOT NULL,
	[Icon] [nvarchar](128) NULL,
	[NotAllowed] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_FileType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO
ALTER TABLE [resources].[FileType] ADD  DEFAULT ((0)) FOR [NotAllowed]
GO

ALTER TABLE [resources].[FileType]  WITH CHECK ADD  CONSTRAINT [FK_FileType_ResourceType] FOREIGN KEY([DefaultResourceTypeId])
REFERENCES [resources].[ResourceType] ([Id])
GO

ALTER TABLE [resources].[FileType] CHECK CONSTRAINT [FK_FileType_ResourceType]
GO