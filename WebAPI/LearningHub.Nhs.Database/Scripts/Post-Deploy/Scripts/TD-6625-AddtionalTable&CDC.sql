-- DROP NodeEditor table
DROP TABLE elfh.userHistoryAttributeTBL

--Enable CDC
EXEC sys.sp_cdc_enable_table
        @source_schema = N'elfh',
        @source_name   = N'userReportingUserTBL',
        @role_name     = NULL,  
        @supports_net_changes = 0;
GO

