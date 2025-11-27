/* 
	TD-6109
	Enable the following tables for CDC / CT in the Learning Hub
*/


-- Enable CDC on the database
EXEC sys.sp_cdc_enable_db;
GO
 
-- STEP 2: Enable CDC on tables
 
-- Table 1: ActivityStatus
EXEC sys.sp_cdc_enable_table
    @source_schema = N'activity',
    @source_name   = N'ActivityStatus',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 2: AssessmentResourceActivity
EXEC sys.sp_cdc_enable_table
    @source_schema = N'activity',
    @source_name   = N'AssessmentResourceActivity',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 3: AssessmentResourceActivity
EXEC sys.sp_cdc_enable_table
    @source_schema = N'activity',
    @source_name   = N'MediaResourceActivity',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 4: ResourceActivity
EXEC sys.sp_cdc_enable_table
    @source_schema = N'activity',
    @source_name   = N'ResourceActivity',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 5: ScormActivity
EXEC sys.sp_cdc_enable_table
    @source_schema = N'activity',
    @source_name   = N'ScormActivity',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 6: CatalogueNodeVersion
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'CatalogueNodeVersion',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 7: FolderNodeVersion
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'FolderNodeVersion',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 8: Node
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'Node',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 9: NodeLink
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'NodeLink',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 10: NodePath
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'NodePath',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 11: NodeResource
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'NodeResource',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 12: NodeVersion
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hierarchy',
    @source_name   = N'NodeVersion',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 13: Role
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'Role',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 14: RoleUserGroup
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'RoleUserGroup',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 15: Scope
EXEC sys.sp_cdc_enable_table
    @source_schema = N'hub',
    @source_name   = N'Scope',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 16: AssessmentResourceVersion
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'AssessmentResourceVersion',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 17: Resource
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'Resource',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 18: ResourceReference
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceReference',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 19: ResourceType
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceType',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 20: ResourceVersion
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceVersion',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 21: ResourceVersionEvent
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceVersionEvent',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 22: ResourceVersionEventType
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceVersionEventType',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 23: VersionStatus
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'VersionStatus',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 24: VideoResourceVersion
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'VideoResourceVersion',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 25: WebLinkResourceVersion
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'WebLinkResourceVersion',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 26: ResourceAccessibility  
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceAccessibility',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 27: ResourceVersionAuthor  
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceVersionAuthor',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 28: ResourceVersionKeyword  
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceVersionKeyword',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 29: ResourceVersionRating  
EXEC sys.sp_cdc_enable_table
    @source_schema = N'resources',
    @source_name   = N'ResourceVersionRating',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO
-- Table 30: ScormActivityInteraction  
EXEC sys.sp_cdc_enable_table
    @source_schema = N'activity',
    @source_name   = N'ScormActivityInteraction',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

-- Table 31: ScormActivityInteractionCorrectResponse  
EXEC sys.sp_cdc_enable_table
    @source_schema = N'activity',
    @source_name   = N'ScormActivityInteractionCorrectResponse',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO





