-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeuserEmployment]
    @userEmploymentList dbo.UserEmployment READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;

    SET IDENTITY_INSERT [elfh].[userEmploymentTBL] ON;
    MERGE [elfh].[userEmploymentTBL] AS target
    USING @userEmploymentList AS source
    ON target.userEmploymentId = source.userEmploymentId

    WHEN MATCHED THEN
        UPDATE SET 
              userId            = source.userId,
              jobRoleId         = source.jobRoleId,
              specialtyId       = source.specialtyId,
              gradeId           = source.gradeId,
              schoolId          = source.schoolId,
              locationId        = source.locationId,
              medicalCouncilId  = source.medicalCouncilId,
              medicalCouncilNo  = source.medicalCouncilNo,
              startDate         = source.startDate,
              endDate           = source.endDate,
              deleted           = source.deleted,
              archived          = source.archived,
              amendUserId       = source.amendUserId,
              amendDate         = source.amendDate

    WHEN NOT MATCHED THEN
        INSERT (
              userEmploymentId,
              userId,
              jobRoleId,
              specialtyId,
              gradeId,
              schoolId,
              locationId,
              medicalCouncilId,
              medicalCouncilNo,
              startDate,
              endDate,
              deleted,
              archived,
              amendUserId,
              amendDate
        )
        VALUES (
              source.userEmploymentId,
              source.userId,
              source.jobRoleId,
              source.specialtyId,
              source.gradeId,
              source.schoolId,
              source.locationId,
              source.medicalCouncilId,
              source.medicalCouncilNo,
              source.startDate,
              source.endDate,
              source.deleted,
              source.archived,
              source.amendUserId,
              source.amendDate
        );

    -- Disable identity insert
    SET IDENTITY_INSERT [elfh].[userEmploymentTBL] OFF;
END
GO
