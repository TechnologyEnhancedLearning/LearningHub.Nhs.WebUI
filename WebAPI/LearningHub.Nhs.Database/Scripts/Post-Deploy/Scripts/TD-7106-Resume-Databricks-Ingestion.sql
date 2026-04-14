IF EXISTS (
    SELECT 1
    FROM sys.change_tracking_databases
    WHERE database_id = DB_ID()
)
BEGIN
    PRINT 'Change Tracking is enabled. Executing setup...';

    EXEC dbo.lakeflowSetupChangeTracking
        @Tables = 'activity.ResourceActivity',  --(to include all other tables with CT enabled)
        @User = 'Elfhadmin',
        @Retention = '2 DAYS';
END
ELSE
BEGIN
    PRINT 'Change Tracking is NOT enabled on this database. Skipping execution.';
END