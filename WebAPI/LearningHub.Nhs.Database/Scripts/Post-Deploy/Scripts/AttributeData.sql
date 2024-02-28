Set Identity_Insert [hub].[Attribute] On
IF NOT EXISTS(SELECT * from [hub].[Attribute] WHERE [Name] = 'Previewer Access User Group')
BEGIN
INSERT INTO [hub].[Attribute]
           (Id
		   ,[AttributeTypeId]
           ,[Name]
           ,[Description]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (8
		   ,2
           ,'Previewer Access User Group'
           ,'Previewer User Group'
           ,0
           ,4
           ,SYSDATETIMEOFFSET()
           ,4
           ,SYSDATETIMEOFFSET())
END
GO
Set Identity_Insert [hub].[Attribute] Off