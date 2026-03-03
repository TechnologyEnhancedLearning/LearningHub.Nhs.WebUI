-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergestaffGroup]
    @staffGroupList dbo.StaffGroup READONLY   -- your table type must exist
AS
BEGIN
    SET NOCOUNT ON;
	SET IDENTITY_INSERT [elfh].[staffGroupTBL] ON;
    MERGE [elfh].[staffGroupTBL] AS target
    USING @staffGroupList AS source
    ON target.staffGroupId = source.staffGroupId

    WHEN MATCHED THEN
        UPDATE SET 
              staffGroupName     = source.staffGroupName
            , displayOrder       = source.displayOrder
            , internalUsersOnly  = source.internalUsersOnly
            , deleted            = source.deleted
            , amendUserID        = source.amendUserID
            , amendDate          = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              staffGroupId
            , staffGroupName
            , displayOrder
            , internalUsersOnly
            , deleted
            , amendUserID
            , amendDate
        )
        VALUES (
              source.staffGroupId
            , source.staffGroupName
            , source.displayOrder
            , source.internalUsersOnly
            , source.deleted
            , source.amendUserID
            , source.amendDate
        );
		SET IDENTITY_INSERT [elfh].[staffGroupTBL] OFF;
END
GO
