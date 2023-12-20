-- Attribute Types
IF NOT EXISTS(SELECT 'X' FROM [hub].[AttributeType] WHERE Id = 1)
BEGIN
INSERT INTO [hub].[AttributeType]([Id],[Name],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (1,'Integer',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [hub].[AttributeType] WHERE Id = 2)
BEGIN
INSERT INTO [hub].[AttributeType]([Id],[Name],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (2,'String',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [hub].[AttributeType] WHERE Id = 3)
BEGIN
INSERT INTO [hub].[AttributeType]([Id],[Name],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (3,'Boolean',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [hub].[AttributeType] WHERE Id = 4)
BEGIN
INSERT INTO [hub].[AttributeType]([Id],[Name],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
    VALUES (4,'DateTime',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END
GO

-- 'Well Known' Attributes
-- 'Well Known' Attributes
SET IDENTITY_INSERT [hub].[Attribute] ON

IF NOT EXISTS(SELECT 'X' FROM [hub].[Attribute] WHERE Id = 1)
BEGIN
	INSERT INTO [hub].[Attribute] ([Id],[AttributeTypeId],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (1, 1,'Local Admin User Group','Local Admin User Group',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [hub].[Attribute] WHERE Id = 2)
BEGIN
	INSERT INTO [hub].[Attribute] ([Id],[AttributeTypeId],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (2, 2,'Restricted Access User Group','Local Admin User Group',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END


SET IDENTITY_INSERT [hub].[Attribute] OFF