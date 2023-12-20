CREATE TABLE [hub].[ExternalOrganisation](
	[Id] [int] NOT NULL,
	[NodeId] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [hub].[ExternalOrganisation]  WITH CHECK ADD  CONSTRAINT [FK_ExternalOrganisation_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hub].[ExternalOrganisation] CHECK CONSTRAINT [FK_ExternalOrganisation_Node]
GO