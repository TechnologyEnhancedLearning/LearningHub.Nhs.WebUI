
-------------------------------------------------------------------------------
-- Author       Colin Beeby
-- Created      21-03-2024
-- Purpose      Ensure that when a user is associated with external system DigitalLearningSolutionsSso, they are also associated with DigitalLearningSolutions
--
-- Modification History
--
-- 21-03-2024  ColB Initial Version
-------------------------------------------------------------------------------

CREATE TRIGGER [external].InsertTrigger_AddDigitalLearningSolutionsExternalSystem
ON [external].[ExternalSystemUser]
FOR INSERT
AS
BEGIN
   DECLARE @DigitalLearningSolutionsId INT;
   DECLARE @DigitalLearningSolutionsSsoId INT;

   SELECT @DigitalLearningSolutionsId = id
    FROM [external].[ExternalSystem]
    WHERE code = 'DigitalLearningSolutions';

   SELECT @DigitalLearningSolutionsSsoId = id
    FROM [external].[ExternalSystem]
    WHERE code = 'DigitalLearningSolutionsSso';


   IF EXISTS (SELECT 1 FROM inserted WHERE ExternalSystemId = @DigitalLearningSolutionsSsoId)
    BEGIN
        INSERT INTO ExternalSystemUser (UserId, ExternalSystemId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
        SELECT UserId, @DigitalLearningSolutionsId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate
        FROM inserted;
    END
END;