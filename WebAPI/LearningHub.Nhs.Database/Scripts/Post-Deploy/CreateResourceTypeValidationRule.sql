IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule] WHERE Id = 7)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule]
           ([Id]
           ,[ResourceTypeId]
           ,[Description]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (7
           ,9
           ,'GenericFile_ValidZipFile'
           ,0
           ,4
           ,SYSDATETIMEOFFSET()
           ,4
           ,SYSDATETIMEOFFSET())
END
GO
IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule] WHERE Id = 8)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule]
           ([Id]
           ,[ResourceTypeId]
           ,[Description]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (8
           ,9
           ,'GenericFile_ValidFileTypes'
           ,0
           ,4
           ,SYSDATETIMEOFFSET()
           ,4
           ,SYSDATETIMEOFFSET())
END
GO
