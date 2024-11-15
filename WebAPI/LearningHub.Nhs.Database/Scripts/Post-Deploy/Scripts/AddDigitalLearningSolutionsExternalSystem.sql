
-------------------------------------------------------------------------------
-- Author       Colin Beeby
-- Created      21-03-2024
-- Purpose      Ensure that when a user is associated with external system DigitalLearningSolutionsSso, they are also associated with DigitalLearningSolutions
--
-- Modification History
--
-- 21-03-2024  ColB Initial Version
-- 15-11-2024  SA   Modified the script to execute check if the trigger exists in the table schema
-------------------------------------------------------------------------------

-- Check if the trigger exists in the [external] schema and the [ExternalSystemUser] table
IF NOT EXISTS (SELECT * FROM sys.triggers WHERE name = 'InsertTrigger_AddDigitalLearningSolutionsExternalSystem' AND parent_id = OBJECT_ID('[external].[ExternalSystemUser]'))
BEGIN
    -- Create the trigger if it doesn't exist
    EXEC('
        CREATE TRIGGER [external].InsertTrigger_AddDigitalLearningSolutionsExternalSystem
        ON [external].[ExternalSystemUser]
        FOR INSERT
        AS
        BEGIN
            DECLARE @DigitalLearningSolutionsId INT;
            DECLARE @DigitalLearningSolutionsSsoId INT;

            -- Get the DigitalLearningSolutions ExternalSystemId
            SELECT @DigitalLearningSolutionsId = id
            FROM [external].[ExternalSystem]
            WHERE code = ''DigitalLearningSolutions'';

            -- Get the DigitalLearningSolutionsSso ExternalSystemId
            SELECT @DigitalLearningSolutionsSsoId = id
            FROM [external].[ExternalSystem]
            WHERE code = ''DigitalLearningSolutionsSso'';

            -- Check if the inserted row has ExternalSystemId matching DigitalLearningSolutionsSsoId
            IF EXISTS (SELECT 1 FROM inserted WHERE ExternalSystemId = @DigitalLearningSolutionsSsoId)
            BEGIN
                -- Insert into ExternalSystemUser table
                INSERT INTO [external].[ExternalSystemUser] 
                    (UserId, ExternalSystemId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
                SELECT UserId, @DigitalLearningSolutionsId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate
                FROM inserted;
            END
        END;
    ');
END;
