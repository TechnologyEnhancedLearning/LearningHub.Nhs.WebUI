-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [AdfMergeMergeUser]
    @MergeUserList dbo.MergeUser READONLY
AS
BEGIN
    SET NOCOUNT ON;

     SET IDENTITY_INSERT [elfh].[mergeUserTBL] ON;

    MERGE [elfh].[mergeUserTBL] AS target
    USING @MergeUserList AS source
        ON target.[mergeUserId] = source.[mergeUserId]

    WHEN MATCHED THEN
        UPDATE SET
            target.[fromUserId]       = source.[fromUserId],
            target.[intoUserId]       = source.[intoUserId],
            target.[amendUserId]      = source.[amendUserId],
            target.[createdDatetime]  = source.[createdDatetime]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [mergeUserId],
            [fromUserId],
            [intoUserId],
            [amendUserId],
            [createdDatetime]
        )
        VALUES (
            source.[mergeUserId],
            source.[fromUserId],
            source.[intoUserId],
            source.[amendUserId],
            source.[createdDatetime]
        );

     SET IDENTITY_INSERT [elfh].[mergeUserTBL] OFF;
END
GO
