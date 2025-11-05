-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergetenantSmtp]
    @tenantSmtpList dbo.TenantSmtp READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [elfh].[tenantSmtpTBL] AS target
    USING @tenantSmtpList AS source
    ON target.tenantId = source.tenantId

    WHEN MATCHED THEN
        UPDATE SET 
              deliveryMethod          = source.deliveryMethod,
              pickupDirectoryLocation = source.pickupDirectoryLocation,
              [from]                  = source.[from],
              userName                = source.userName,
              [password]              = source.[password],
              enableSsl               = source.enableSsl,
              host                    = source.host,
              port                    = source.port,
              active                  = source.active,
              deleted                 = source.deleted,
              amendUserId             = source.amendUserId,
              amendDate               = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              tenantId,
              deliveryMethod,
              pickupDirectoryLocation,
              [from],
              userName,
              [password],
              enableSsl,
              host,
              port,
              active,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.tenantId,
              source.deliveryMethod,
              source.pickupDirectoryLocation,
              source.[from],
              source.userName,
              source.[password],
              source.enableSsl,
              source.host,
              source.port,
              source.active,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );

END
GO
