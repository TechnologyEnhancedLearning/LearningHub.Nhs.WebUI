-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeUserProfileData]
    @UserProfileLists [dbo].[UserProfileType] READONLY
AS
BEGIN
    SET NOCOUNT ON;

        MERGE [hub].[UserProfile] AS target
        USING @UserProfileLists AS source
        ON target.[Id] = source.[Id]

        WHEN MATCHED THEN
            UPDATE SET
                target.[UserName]         = source.[UserName],
                target.[EmailAddress]     = source.[EmailAddress],
                target.[AltEmailAddress]  = source.[AltEmailAddress],
                target.[FirstName]        = source.[FirstName],
                target.[LastName]         = source.[LastName],
                target.[PreferredName]    = source.[PreferredName],
                target.[Active]           = source.[Active],
                target.[AmendUserId]      = source.[AmendUserId],
                target.[AmendDate]        = source.[AmendDate],
                target.[Deleted]          = source.[Deleted]

        WHEN NOT MATCHED BY TARGET THEN
            INSERT (
                [Id],
                [UserName],
                [EmailAddress],
                [AltEmailAddress],
                [FirstName],
                [LastName],
                [PreferredName],
                [Active],
                [CreateUserId],
                [CreateDate],
                [AmendUserId],
                [AmendDate],
                [Deleted]
            )
            VALUES (
                source.[Id],
                source.[UserName],
                source.[EmailAddress],
                source.[AltEmailAddress],
                source.[FirstName],
                source.[LastName],
                source.[PreferredName],
                source.[Active],
                4,
                source.[CreateDate],
                source.[AmendUserId],
                source.[AmendDate],
                source.[Deleted]
            );



END;
GO
