-- Disable CDC
IF EXISTS (
    SELECT 1
    FROM sys.change_data_capture_tables
    WHERE source_object_id = OBJECT_ID(N'hub.User')
)
BEGIN
    EXEC sys.sp_cdc_disable_table
        @source_schema = N'hub',
        @source_name   = N'User',
        @capture_instance = N'hub_User';
END
GO

IF EXISTS (
    SELECT 1
    FROM sys.change_data_capture_tables
    WHERE source_object_id = OBJECT_ID(N'hub.UserProfile')
)
BEGIN
    EXEC sys.sp_cdc_disable_table
        @source_schema = N'hub',
        @source_name   = N'UserProfile',
        @capture_instance = N'hub_UserProfile';
END
GO
