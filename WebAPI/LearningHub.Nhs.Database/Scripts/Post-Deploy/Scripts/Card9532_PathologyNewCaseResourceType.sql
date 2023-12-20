IF NOT EXISTS (SELECT 1 FROM [resources].[ResourceType] WHERE Id = 10)
BEGIN
	INSERT INTO [resources].[ResourceType]
	    ([Id], [Name], [Description], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted])
    VALUES (10, 'Case', '', 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET(), 0)
END