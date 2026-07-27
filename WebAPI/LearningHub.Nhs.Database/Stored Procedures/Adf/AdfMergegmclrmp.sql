-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergegmclrmp]
    @gmclrmpList dbo.GMCLRMP READONLY   -- Table-valued parameter type
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [elfh].[gmclrmpTBL] AS target
    USING @gmclrmpList AS source
    ON target.GMC_Ref_No = source.GMC_Ref_No

    WHEN MATCHED THEN
        UPDATE SET 
              Surname               = source.Surname,
              Given_Name            = source.Given_Name,
              Year_Of_Qualification = source.Year_Of_Qualification,
              GP_Register_Date      = source.GP_Register_Date,
              Registration_Status   = source.Registration_Status,
              Other_Names           = source.Other_Names,
              dateProcessed         = source.dateProcessed,
              action                = source.action

    WHEN NOT MATCHED THEN
        INSERT (
              GMC_Ref_No,
              Surname,
              Given_Name,
              Year_Of_Qualification,
              GP_Register_Date,
              Registration_Status,
              Other_Names,
              dateProcessed,
              action
        )
        VALUES (
              source.GMC_Ref_No,
              source.Surname,
              source.Given_Name,
              source.Year_Of_Qualification,
              source.GP_Register_Date,
              source.Registration_Status,
              source.Other_Names,
              source.dateProcessed,
              source.action
        );

END
GO
