IF NOT EXISTS(SELECT Id FROM [resources].[FileType] where Extension ='ppsx')
BEGIN
INSERT INTO [resources].[FileType]
           (Id,
            [DefaultResourceTypeId]
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
           (70,
           9
           ,'PowerPoint Open XML Slide Show'
           ,'PowerPoint Open XML Slide Show'
           ,'ppsx'
           ,'a-mppoint-icon.svg'
           ,0
           ,0
           ,57541
           ,SYSDATETIMEOFFSET()
           ,57541
           ,SYSDATETIMEOFFSET())
END
GO