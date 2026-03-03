-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergemedicalCouncil]
    @medicalCouncilList dbo.MedicalCouncil READONLY
AS
BEGIN
    SET NOCOUNT ON;
    MERGE [elfh].[medicalCouncilTBL] AS target
    USING @medicalCouncilList AS source
    ON target.medicalCouncilId = source.medicalCouncilId
    WHEN MATCHED THEN
        UPDATE SET 
			medicalCouncilName		= source.medicalCouncilName
		  ,medicalCouncilCode		= source.medicalCouncilCode
		  ,uploadPrefix				= source.uploadPrefix
		  ,includeOnCerts			= source.includeOnCerts
		  ,deleted					= source.deleted
		  ,amendUserID				= source.amendUserID
		  ,amendDate				= source.amendDate
           
    WHEN NOT MATCHED THEN
        INSERT (
					medicalCouncilId
           			,medicalCouncilName	
				  ,medicalCouncilCode	
				  ,uploadPrefix			
				  ,includeOnCerts		
				  ,deleted				
				  ,amendUserID			
				  ,amendDate			
        )
        VALUES (
                  source.medicalCouncilId
           		  ,source.medicalCouncilName	
				  ,source.medicalCouncilCode	
				  ,source.uploadPrefix			
				  ,source.includeOnCerts		
				  ,source.deleted				
				  ,source.amendUserID			
				  ,source.amendDate
        );
END
GO