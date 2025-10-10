-- Re-enable CDC
IF NOT EXISTS (
    SELECT 1
    FROM sys.change_data_capture_tables
    WHERE source_object_id = OBJECT_ID(N'hub.User')
)
BEGIN
    EXEC sys.sp_cdc_enable_table
        @source_schema = N'hub',
        @source_name   = N'User',
        @role_name     = NULL,  
        @supports_net_changes = 1;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.change_data_capture_tables
    WHERE source_object_id = OBJECT_ID(N'hub.UserProfile')
)
BEGIN
    EXEC sys.sp_cdc_enable_table
        @source_schema = N'hub',
        @source_name   = N'UserProfile',
        @role_name     = NULL,
        @supports_net_changes = 1;
END
GO
