CREATE TABLE [resources].[EquipmentResourceVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[ContactName] [nvarchar](255) NULL,
	[ContactTelephone] [nvarchar](255) NULL,
	[ContactEmail] [nvarchar](255) NULL,
	[AddressId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_EquipmentResourceVersion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [resources].[EquipmentResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_EquipmentResourceVersion_Address] FOREIGN KEY([AddressId])
REFERENCES [hub].[Address] ([Id])
GO

ALTER TABLE [resources].[EquipmentResourceVersion] CHECK CONSTRAINT [FK_EquipmentResourceVersion_Address]
GO

ALTER TABLE [resources].[EquipmentResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_EquipmentResourceVersion_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[EquipmentResourceVersion] CHECK CONSTRAINT [FK_EquipmentResourceVersion_ResourceVersion]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_EquipmentResourceVersion]
    ON [resources].[EquipmentResourceVersion]([ResourceVersionId] ASC);
GO