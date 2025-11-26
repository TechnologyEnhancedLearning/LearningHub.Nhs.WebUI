-- Re-enable CDC

EXEC sys.sp_cdc_enable_table
        @source_schema = N'hub',
        @source_name   = N'User',
        @role_name     = NULL,  
        @supports_net_changes = 0;
GO


EXEC sys.sp_cdc_enable_table
        @source_schema = N'hub',
        @source_name   = N'UserProfile',
        @role_name     = NULL,
        @supports_net_changes = 0;
GO


-- Enable CDC for elfh tables

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'locationTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'locationTypeTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'countryTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'userEmploymentTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'jobRoleTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'StaffGroupTbL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'specialtyTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'medicalCouncilTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'userEmploymentReferenceTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'employmentReferenceTypeTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'userAdminLocationTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'gradeTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'userEmploymentResponsibilityTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'mergeUserTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO

EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'userReportingUserTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO


EXEC sys.sp_cdc_enable_table
    @source_schema = N'elfh',
    @source_name   = N'userTermsAndConditionsTBL',
    @role_name     = NULL,
    @supports_net_changes = 0;
GO



