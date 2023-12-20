CREATE TABLE [resources].[UrlRewriting](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalReferenceId] [int] NOT NULL,
	[FullHistoricUrl] [nvarchar](4000) NOT NULL,
	[PackageRootUrl] [nvarchar](4000) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_UrlRewriting] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [resources].[UrlRewriting]  WITH CHECK ADD  CONSTRAINT [FK_UrlRewriting_ExternalReference] FOREIGN KEY([ExternalReferenceId])
REFERENCES [resources].[ExternalReference] ([Id])
GO

ALTER TABLE [resources].[UrlRewriting] CHECK CONSTRAINT [FK_UrlRewriting_ExternalReference]
GO