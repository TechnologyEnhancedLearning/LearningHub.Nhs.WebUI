-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE  PROCEDURE [dbo].[AdfMergegrade]
    @gradeList dbo.Grade READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;
	SET IDENTITY_INSERT [elfh].[gradeTBL] ON;
    MERGE [elfh].[gradeTBL] AS target
    USING @gradeList AS source
    ON target.gradeId = source.gradeId

    WHEN MATCHED THEN
        UPDATE SET 
              gradeName   = source.gradeName
            , displayOrder = source.displayOrder
            , deleted      = source.deleted
            , amendUserID  = source.amendUserID
            , amendDate    = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              gradeId
            , gradeName
            , displayOrder
            , deleted
            , amendUserID
            , amendDate
        )
        VALUES (
              source.gradeId
            , source.gradeName
            , source.displayOrder
            , source.deleted
            , source.amendUserID
            , source.amendDate
        );
		SET IDENTITY_INSERT [elfh].[gradeTBL] OFF;
END
GO
