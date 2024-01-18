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

IF NOT EXISTS (SELECT 1 FROM [external].[ExternalSystem] WHERE [Code] = 'DigitalLearningSolutionsSso')
BEGIN
INSERT INTO [external].[ExternalSystem]
           ([Name]
           ,[Code]
           ,[CallbackUrl]
           ,[SecretKey]
           ,[TermsAndConditions]
           ,[DefaultUserGroupId]
           ,[DefaultStaffGroupId]
           ,[DefaultJobRoleId]
           ,[DefaultGradingId]
           ,[DefaultSpecialityId]
           ,[DefaultLocationId]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           ('Digital Learning Solutions'
           ,'DigitalLearningSolutionsSso'
           ,'https://www.dls.nhs.uk/v2/linkaccount/accountlinked'
           ,'74A90C2B-5BC5-4877-8607-43AD08DEA983'
           ,'By clicking the button below, you agree to the Learning Hub and Digital Learning Solutions creating a link between your accounts on their systems so that you can log in to the Learning Hub seamlessly from the Digital Learning Solutions site via Single Sign On. Do you agree to the Learning Hub and Digital Learning Solutions linking your accounts?'
           ,'1070'
           ,null
           ,null
           ,null
           ,227
           ,1
           ,0
           ,57541
           ,SYSDATETIMEOFFSET()
           ,57541
           ,SYSDATETIMEOFFSET())
END
