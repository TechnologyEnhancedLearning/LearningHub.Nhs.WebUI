
IF EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 6)
BEGIN
	UPDATE [resources].[ResourceType]  SET [Name] = 'SCORM/AICC e-learning resource', AmendDate = SYSDATETIMEOFFSET()   WHERE Id = 6
END


IF EXISTS(SELECT 1 FROM [resources].[ResourceType] WHERE Id = 8)
BEGIN
	UPDATE [resources].[ResourceType]   SET  [Name] = 'Web link' WHERE Id = 8
END

GO

