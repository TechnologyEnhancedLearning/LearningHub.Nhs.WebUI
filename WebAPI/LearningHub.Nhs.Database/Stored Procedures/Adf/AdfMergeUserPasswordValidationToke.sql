-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserPasswordValidationToken]
    @userPasswordValidationTokenList dbo.UserPasswordValidationToken READONLY
AS
BEGIN
    SET NOCOUNT ON;

    -- Enable identity insert if userPasswordValidationTokenId is an IDENTITY column
    SET IDENTITY_INSERT [elfh].[userPasswordValidationTokenTBL] ON;
	ALTER TABLE [elfh].[userPasswordValidationTokenTBL] NOCHECK CONSTRAINT ALL;
	ALTER TABLE [hub].[User] NOCHECK CONSTRAINT ALL;
    MERGE [elfh].[userPasswordValidationTokenTBL] AS target
    USING @userPasswordValidationTokenList AS source
    ON target.userPasswordValidationTokenId = source.userPasswordValidationTokenId

    WHEN MATCHED THEN
        UPDATE SET
              hashedToken   = source.hashedToken,
              salt          = source.salt,
              [lookup]      = source.[lookup],
              expiry        = source.expiry,
              tenantId      = source.tenantId,
              userId        = source.userId,
              createdUserId = source.createdUserId,
              createdDate   = source.createdDate

    WHEN NOT MATCHED THEN
        INSERT (
              userPasswordValidationTokenId,
              hashedToken,
              salt,
              [lookup],
              expiry,
              tenantId,
              userId,
              createdUserId,
              createdDate
        )
        VALUES (
              source.userPasswordValidationTokenId,
              source.hashedToken,
              source.salt,
              source.[lookup],
              source.expiry,
              source.tenantId,
              source.userId,
              source.createdUserId,
              source.createdDate
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[userPasswordValidationTokenTBL] OFF;
	ALTER TABLE [hub].[User] CHECK CONSTRAINT ALL;
	ALTER TABLE [elfh].[userPasswordValidationTokenTBL] CHECK CONSTRAINT ALL;
END
GO
