DECLARE @now datetimeoffset(7) = SYSDATETIMEOFFSET()


IF NOT EXISTS(SELECT Id FROM [resources].[ResourceType] WHERE Id = 12)
BEGIN
	INSERT INTO [resources].[ResourceType]
	VALUES (12, 'Html', '', 0, 4, @now, 4, @now)
END

IF NOT EXISTS(SELECT Id FROM [resources].[ResourceTypeValidationRule] WHERE Id = 6)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule]
VALUES (6, 12, 'HtmlResource_RootIndexPresent', 0, 4, @now, 4, @now)
END

UPDATE resources.FileType SET NotAllowed = 1 WHERE Extension IN ('odp','key','pages','numbers','dhtml','odt')
UPDATE [resources].[FileType] SET Deleted = 1 WHERE Id =0 


IF NOT EXISTS(SELECT Id FROM [resources].[FileType] WHERE Extension = 'pub')
BEGIN
INSERT INTO [resources].[FileType]
           ([Id]
           ,[DefaultResourceTypeId]
           ,[Name]
           ,[Description]
           ,[Extension]
           ,[Icon]
           ,[NotAllowed]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (69
           ,9
           ,'Microsoft Publisher'
           ,'Microsoft Publisher'
           ,'pub'
           ,NULL
           ,0
           ,0
           ,57541
           ,SYSDATETIMEOFFSET()
           ,57541
           ,SYSDATETIMEOFFSET())
END
GO


