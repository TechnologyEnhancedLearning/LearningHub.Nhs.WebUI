IF NOT EXISTS (SELECT * FROM [hub].[Role] WHERE id = 9)
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
           (9,
		   'Course Creator'
           ,'Course Creator Role'
           ,0
           ,4
           ,SYSDATETIMEOFFSET()
           ,4
           ,SYSDATETIMEOFFSET())
	SET IDENTITY_INSERT [hub].[Role] OFF
END
