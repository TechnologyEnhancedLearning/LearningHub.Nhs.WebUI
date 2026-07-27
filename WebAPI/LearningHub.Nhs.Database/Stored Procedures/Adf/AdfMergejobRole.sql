-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergejobRole]
    @jobRoleList dbo.JobRole READONLY   -- table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;
	SET IDENTITY_INSERT [elfh].[jobRoleTBL] ON;
    MERGE [elfh].[jobRoleTBL] AS target
    USING @jobRoleList AS source
    ON target.jobRoleId = source.jobRoleId

    WHEN MATCHED THEN
        UPDATE SET 
              staffGroupId     = source.staffGroupId
            , jobRoleName      = source.jobRoleName
            , medicalCouncilId = source.medicalCouncilId
            , displayOrder     = source.displayOrder
            , deleted          = source.deleted
            , amendUserID      = source.amendUserID
            , amendDate        = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              jobRoleId
            , staffGroupId
            , jobRoleName
            , medicalCouncilId
            , displayOrder
            , deleted
            , amendUserID
            , amendDate
        )
        VALUES (
              source.jobRoleId
            , source.staffGroupId
            , source.jobRoleName
            , source.medicalCouncilId
            , source.displayOrder
            , source.deleted
            , source.amendUserID
            , source.amendDate
        );
		SET IDENTITY_INSERT [elfh].[jobRoleTBL] OFF;
END
GO
