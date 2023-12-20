IF NOT EXISTS(select CODE from [external].[ExternalSystemDeepLink] WHERE Code = 'elfh_dashboard')
BEGIN	
INSERT INTO [external].[ExternalSystemDeepLink]
           ([Code]
           ,[DeepLink]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           ('elfh_dashboard'
           ,'https://test-portal.e-lfhtech.org.uk/Dashboard'
           ,0
           ,57541
           ,SYSDATETIMEOFFSET()
           ,57541
           ,SYSDATETIMEOFFSET())
END
