CREATE TABLE [hub].[Tenant](
	[Id] [int] NOT NULL,
	[Code] [varchar](8) NULL,
	[Name] [varchar](100) NULL,
	[Description] [varchar](1024) NULL,
	[ShowFullCatalogInfoMessageInd] [bit] NOT NULL,
	[CatalogUrl] [nvarchar](256) NOT NULL,
	[QuickStartGuideUrl] [nvarchar](1024) NULL,
	[SupportFormUrl] [nvarchar](1024) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_tenant] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)

GO

----Add extended property for table 
EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Table to define tenant, stores tenant information' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant' 
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Technical Description', @value=N'Tenant name required for internal reference' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'Name'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Technical Description', @value=N'Tenant code to identify the tenant and load respective tenant url' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'Code'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Business Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'Code'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Technical Description', @value=N'Summary about the tenant' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'Description'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Business Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'Description'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Technical Description', @value=N'Quick Start Guide Url of the tenant' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'QuickStartGuideUrl'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Business Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'QuickStartGuideUrl'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Technical Description', @value=N'Support Form Url' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'SupportFormUrl'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Business Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'Tenant', @level2type=N'COLUMN',@level2name=N'SupportFormUrl'
GO

