-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergegdcRegister]
    @gdcRegisterList dbo.GDCRegister READONLY   -- Table-valued parameter
AS
BEGIN
    SET NOCOUNT ON;

    MERGE [ELFH].[gdcRegisterTBL] AS target
    USING @gdcRegisterList AS source
    ON target.reg_number = source.reg_number

    WHEN MATCHED THEN
        UPDATE SET 
              Dentist         = source.Dentist,
              Title           = source.Title,
              Surname         = source.Surname,
              Forenames       = source.Forenames,
              honorifics      = source.honorifics,
              house_name      = source.house_name,
              address_line1   = source.address_line1,
              address_line2   = source.address_line2,
              address_line3   = source.address_line3,
              address_line4   = source.address_line4,
              Town            = source.Town,
              County          = source.County,
              PostCode        = source.PostCode,
              Country         = source.Country,
              regdate         = source.regdate,
              qualifications  = source.qualifications,
              dcp_titles      = source.dcp_titles,
              specialties     = source.specialties,
              [condition]     = source.[condition],
              suspension      = source.suspension,
              dateProcessed   = source.dateProcessed,
              action          = source.action

    WHEN NOT MATCHED THEN
        INSERT (
              reg_number,
              Dentist,
              Title,
              Surname,
              Forenames,
              honorifics,
              house_name,
              address_line1,
              address_line2,
              address_line3,
              address_line4,
              Town,
              County,
              PostCode,
              Country,
              regdate,
              qualifications,
              dcp_titles,
              specialties,
              [condition],
              suspension,
              dateProcessed,
              action
        )
        VALUES (
              source.reg_number,
              source.Dentist,
              source.Title,
              source.Surname,
              source.Forenames,
              source.honorifics,
              source.house_name,
              source.address_line1,
              source.address_line2,
              source.address_line3,
              source.address_line4,
              source.Town,
              source.County,
              source.PostCode,
              source.Country,
              source.regdate,
              source.qualifications,
              source.dcp_titles,
              source.specialties,
              source.[condition],
              source.suspension,
              source.dateProcessed,
              source.action
        );

END
GO
