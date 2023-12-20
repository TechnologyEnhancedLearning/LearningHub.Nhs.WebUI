CREATE TABLE [resources].[ExternalReference](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceReferenceId] [int] NOT NULL,
	[ExternalReference] [nvarchar](255) NOT NULL,
	[Active] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_ExternalReference] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [resources].[ExternalReference]  WITH CHECK ADD  CONSTRAINT [FK_ExternalReference_ResourceReference] FOREIGN KEY([ResourceReferenceId])
REFERENCES [resources].[ResourceReference] ([Id])
GO

ALTER TABLE [resources].[ExternalReference] CHECK CONSTRAINT [FK_ExternalReference_ResourceReference]
GO