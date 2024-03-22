/*
Script for adding DigitalLearningSolutionsSso ExternalSystemUser to users which already have DigitalLearningSolutions ExternalSystemUser
*/
DECLARE @DigitalLearningSolutionsId INT;
DECLARE @DigitalLearningSolutionsSsoId INT;

SELECT @DigitalLearningSolutionsId = id
  FROM [external].[ExternalSystem]
  WHERE code = 'DigitalLearningSolutions';

SELECT @DigitalLearningSolutionsSsoId = id
  FROM [external].[ExternalSystem]
  WHERE code = 'DigitalLearningSolutionsSso';

INSERT INTO [external].[ExternalSystemUser] (UserId, ExternalSystemId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
SELECT UserId, @DigitalLearningSolutionsId, Deleted, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()
FROM [external].[ExternalSystemUser]
WHERE ExternalSystemId = @DigitalLearningSolutionsSsoId
  AND userId NOT IN (
    SELECT userId
    FROM [external].[ExternalSystemUser]
    WHERE ExternalSystemId = @DigitalLearningSolutionsId
  );