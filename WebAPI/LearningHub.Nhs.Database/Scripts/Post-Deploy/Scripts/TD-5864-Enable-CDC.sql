/* 
	TD-5864/TD-6220/TD-6366
	Enable the following tables for CDC / CT in the Learning Hub
*/


-- Enable CDC on the database
EXEC sys.sp_cdc_enable_db;
GO
 
-- STEP 2: Enable CDC on tables
 
-- Table 1: UserUserGroup
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'UserUserGroup',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 2: UserGroup
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'UserGroup',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 3: UserProvider
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'UserProvider',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 4: UserGroupAttribute
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'UserGroupAttribute',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 5: Attribute
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'Attribute',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 6: AttributeType
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'AttributeType',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 7: NodeType
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'NodeType',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 8: CatalogueNodeVersionProvider
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'CatalogueNodeVersionProvider',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 9: CatalogueNodeVersionKeyword
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'CatalogueNodeVersionKeyword',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 10: Publication
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'Publication',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 11: PublicationLog
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'PublicationLog',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 12: NodePathNode
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'NodePathNode',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 12: versionStatus
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'versionStatus',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO















