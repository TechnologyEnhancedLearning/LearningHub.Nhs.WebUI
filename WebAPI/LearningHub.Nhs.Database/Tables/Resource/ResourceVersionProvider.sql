CREATE TABLE [resources].[ResourceVersionProvider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[ProviderId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_resourceVersionProvider] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [resources].[ResourceVersionProvider]  WITH CHECK ADD  CONSTRAINT [FK_resourceVersionProvider_resourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionProvider] CHECK CONSTRAINT [FK_resourceVersionProvider_resourceVersion]
GO

ALTER TABLE [resources].[ResourceVersionProvider]  WITH CHECK ADD  CONSTRAINT [FK_resourceVersionProvider_provider] FOREIGN KEY([ProviderId])
REFERENCES [hub].[Provider] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionProvider] CHECK CONSTRAINT [FK_resourceVersionProvider_provider]
GO