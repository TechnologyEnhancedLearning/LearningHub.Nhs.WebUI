IF NOT EXISTS (SELECT * FROM [hub].[Role] WHERE id = 8)
BEGIN
	SET IDENTITY_INSERT [hub].[Role] ON
	INSERT INTO [hub].[Role]
           ([Id],
		    [Name]
           ,[Description]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (8,
		   'Previewer'
           ,'Previewer Role'
           ,0
           ,4
           ,SYSDATETIMEOFFSET()
           ,4
           ,SYSDATETIMEOFFSET())
	SET IDENTITY_INSERT [hub].[Role] OFF
END
