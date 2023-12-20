CREATE TABLE [hub].[TenantUrl](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TenantId] [int] NOT NULL,
	[UrlHostName] [nvarchar](128) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_tenantUrl] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[TenantUrl]  WITH NOCHECK ADD  CONSTRAINT [FK_tenantUrl_tenant] FOREIGN KEY([TenantId])
REFERENCES [hub].[Tenant] ([Id])
GO

----Add extended property for table 
EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Table to define tenant Url (hostname/domain name)' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'TenantUrl' 
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Technical Description', @value=N'Url (Domain name) to identify Tenant' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'TenantUrl', @level2type=N'COLUMN',@level2name=N'UrlHostName'
GO