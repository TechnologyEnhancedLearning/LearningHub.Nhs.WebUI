-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergetenantsUrl]
    @tenantUrlList dbo.TenantUrl READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    -- Enable identity insert if tenantUrlId is an IDENTITY column
    SET IDENTITY_INSERT [elfh].[tenantUrlTBL] ON;

    MERGE [elfh].[tenantUrlTBL] AS target
    USING @tenantUrlList AS source
    ON target.tenantUrlId = source.tenantUrlId

    WHEN MATCHED THEN
        UPDATE SET 
              tenantId       = source.tenantId,
              urlHostName    = source.urlHostName,
              useHostForAuth = source.useHostForAuth,
              deleted        = source.deleted,
              amendUserID    = source.amendUserID,
              amendDate      = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              tenantUrlId,
              tenantId,
              urlHostName,
              useHostForAuth,
              deleted,
              amendUserID,
              amendDate
        )
        VALUES (
              source.tenantUrlId,
              source.tenantId,
              source.urlHostName,
              source.useHostForAuth,
              source.deleted,
              source.amendUserID,
              source.amendDate
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[tenantUrlTBL] OFF;
END
GO
