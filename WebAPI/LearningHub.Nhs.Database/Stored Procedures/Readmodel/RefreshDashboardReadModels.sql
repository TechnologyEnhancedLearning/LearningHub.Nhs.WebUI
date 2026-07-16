-------------------------------------------------------------------------------
-- Author       OA
-- Created      23 April 2026
-- Purpose      Refresh resource and calaogue read models
--
-- Modification History
-- 
-- 23 April 2026  OA  TD-7078 Script Optimization
-------------------------------------------------------------------------------
CREATE PROCEDURE [readmodels].[RefreshDashboardReadModels]
AS
BEGIN
    SET NOCOUNT ON;
    SET XACT_ABORT ON;

    DECLARE @LockResult INT;

    BEGIN TRAN;

    EXEC @LockResult = sys.sp_getapplock
        @Resource = 'readmodels.RefreshDashboardReadModels',
        @LockMode = 'Exclusive',
        @LockOwner = 'Transaction',
        @LockTimeout = 0;

    IF @LockResult < 0
    BEGIN
        -- If already running,exit gracefully.
        ROLLBACK TRAN;
        RETURN;
    END;

    BEGIN TRY
        EXEC [readmodels].[RefreshDashboardResources];
        EXEC [readmodels].[RefreshDashboardCatalogues];

        COMMIT TRAN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRAN;

        THROW;
    END CATCH
END;
GO