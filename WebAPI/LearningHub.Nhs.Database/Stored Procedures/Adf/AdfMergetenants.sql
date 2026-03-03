-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergetenants]
    @tenantList dbo.Tenant READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [elfh].[tenantTBL] AS target
    USING @tenantList AS source
    ON target.tenantId = source.tenantId

    WHEN MATCHED THEN
        UPDATE SET 
              tenantCode                       = source.tenantCode,
              tenantName                       = source.tenantName,
              tenantDescription                = source.tenantDescription,
              showFullCatalogInfoMessageInd     = source.showFullCatalogInfoMessageInd,
              catalogUrl                        = source.catalogUrl,
              quickStartGuideUrl                = source.quickStartGuideUrl,
              supportFormUrl                    = source.supportFormUrl,
              liveChatStatus                    = source.liveChatStatus,
              liveChatSnippet                   = source.liveChatSnippet,
              myElearningDefaultView            = source.myElearningDefaultView,
              preLoginCatalogueDefaultView      = source.preLoginCatalogueDefaultView,
              postLoginCatalogueDefaultView     = source.postLoginCatalogueDefaultView,
              authSignInUrlRelative             = source.authSignInUrlRelative,
              authSignOutUrlRelative            = source.authSignOutUrlRelative,
              authSecret                        = source.authSecret,
              deleted                           = source.deleted,
              amendUserId                       = source.amendUserId,
              amendDate                         = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              tenantId,
              tenantCode,
              tenantName,
              tenantDescription,
              showFullCatalogInfoMessageInd,
              catalogUrl,
              quickStartGuideUrl,
              supportFormUrl,
              liveChatStatus,
              liveChatSnippet,
              myElearningDefaultView,
              preLoginCatalogueDefaultView,
              postLoginCatalogueDefaultView,
              authSignInUrlRelative,
              authSignOutUrlRelative,
              authSecret,
              deleted,
              amendUserId,
              amendDate
        )
        VALUES (
              source.tenantId,
              source.tenantCode,
              source.tenantName,
              source.tenantDescription,
              source.showFullCatalogInfoMessageInd,
              source.catalogUrl,
              source.quickStartGuideUrl,
              source.supportFormUrl,
              source.liveChatStatus,
              source.liveChatSnippet,
              source.myElearningDefaultView,
              source.preLoginCatalogueDefaultView,
              source.postLoginCatalogueDefaultView,
              source.authSignInUrlRelative,
              source.authSignOutUrlRelative,
              source.authSecret,
              source.deleted,
              source.amendUserId,
              source.amendDate
        );

END
GO
