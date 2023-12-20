-- Insert [hub].[Attribute] definition.

IF NOT EXISTS(SELECT 1 FROM [hub].[Attribute] WHERE Name = 'Resource MigrationInputRecordId')
BEGIN
	INSERT INTO [hub].[Attribute] ([Id],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (3, 'Resource MigrationInputRecordId','If a resource was created during a migration, this attribute defines the Id of the MigrationInputRecord that it was originally created from.',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END

GO