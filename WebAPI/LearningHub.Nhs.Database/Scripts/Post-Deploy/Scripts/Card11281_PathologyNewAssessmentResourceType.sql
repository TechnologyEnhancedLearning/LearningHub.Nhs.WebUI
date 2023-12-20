IF NOT EXISTS (SELECT 1 FROM [resources].[ResourceType] WHERE Id = 11)
BEGIN
    INSERT INTO [resources].[ResourceType]
        ([Id], [Name], [Description], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted])
    VALUES (11, 'Assessment', '', 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET(), 0)
END