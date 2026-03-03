
EXEC sys.sp_cdc_disable_table
        @source_schema = N'hub',
        @source_name   = N'User',
        @capture_instance = N'hub_User';
GO


EXEC sys.sp_cdc_disable_table
        @source_schema = N'hub',
        @source_name   = N'UserProfile',
        @capture_instance = N'hub_UserProfile';
GO
