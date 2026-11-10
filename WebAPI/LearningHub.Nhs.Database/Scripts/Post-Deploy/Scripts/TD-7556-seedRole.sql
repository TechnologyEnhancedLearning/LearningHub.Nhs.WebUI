DECLARE @CreateDate DATETIMEOFFSET(7) = SYSDATETIMEOFFSET();

UPDATE [hub].[Role]
SET
    [Code] = N'catalogue_editor',
    [Name] = N'Catalogue editor',
    [ScopeType] = N'catalogue',
    [CreateDate] = COALESCE([CreateDate], @CreateDate)
WHERE [Name] = N'Editor'
   OR [Code] IN (N'editor', N'catalogue_editor');

UPDATE [hub].[Role]
SET
    [Code] = N'catalogue_reader',
    [Name] = N'Catalogue reader',
    [ScopeType] = N'catalogue',
    [CreateDate] = COALESCE([CreateDate], @CreateDate)
WHERE [Name] = N'Reader'
   OR [Code] IN (N'reader', N'catalogue_reader');

UPDATE [hub].[Role]
SET
    [Code] = N'catalogue_admin',
    [Name] = N'Catalogue admin',
    [ScopeType] = N'catalogue',
    [CreateDate] = COALESCE([CreateDate], @CreateDate)
WHERE [Name] = N'Local Admin'
   OR [Code] = N'catalogue_admin';

UPDATE [hub].[Role]
SET
    [Code] = N'debugger',
    [ScopeType] = N'service',
    [CreateDate] = COALESCE([CreateDate], @CreateDate)
WHERE [Name] = N'Debugger'
   OR [Code] = N'debugger';

UPDATE [hub].[Role]
SET
    [Code] = N'release_tester',
    [ScopeType] = N'service',
    [CreateDate] = COALESCE([CreateDate], @CreateDate)
WHERE [Name] = N'Release tester'
   OR [Code] = N'release_tester';

UPDATE [hub].[Role]
SET
    [Code] = N'release_manager',
    [ScopeType] = N'service',
    [CreateDate] = COALESCE([CreateDate], @CreateDate)
WHERE [Name] = N'Release manager'
   OR [Code] = N'release_manager';

UPDATE [hub].[Role]
SET
    [Code] = N'reporter',
    [ScopeType] = N'multi',
    [CreateDate] = COALESCE([CreateDate], @CreateDate)
WHERE [Name] = N'Reporter'
   OR [Code] = N'reporter';

UPDATE [hub].[Role]
SET
    [Code] = N'catalogue_previewer',
    [Name] = N'Catalogue previewer',
    [ScopeType] = N'catalogue',
    [CreateDate] = COALESCE([CreateDate], @CreateDate)
WHERE [Name] = N'Previewer'
   OR [Code] IN (N'previewer', N'catalogue_previewer');


IF NOT EXISTS
(
    SELECT 1
    FROM [hub].[Role]
    WHERE [Code] = N'report_viewer'
       OR [Name] = N'Report viewer'
)
BEGIN
    INSERT INTO [hub].[Role]
    (
        [Code],
        [Name],
        [ScopeType],
        [Description],
        [CreateDate]
    )
    VALUES
    (
        N'report_viewer',
        N'Report viewer',
        N'multi',
        NULL,
        @CreateDate
    );
END;


IF NOT EXISTS
(
    SELECT 1
    FROM [hub].[Role]
    WHERE [Code] = N'catalogue_owner'
       OR [Name] = N'Catalogue owner'
)
BEGIN
    INSERT INTO [hub].[Role]
    (
        [Code],
        [Name],
        [ScopeType],
        [Description],
        [CreateDate]
    )
    VALUES
    (
        N'catalogue_owner',
        N'Catalogue owner',
        N'catalogue',
        NULL,
        @CreateDate
    );
END;


IF NOT EXISTS
(
    SELECT 1
    FROM [hub].[Role]
    WHERE [Code] = N'platform_admin'
       OR [Name] = N'Platform administrator'
)
BEGIN
    INSERT INTO [hub].[Role]
    (
        [Code],
        [Name],
        [ScopeType],
        [Description],
        [CreateDate]
    )
    VALUES
    (
        N'platform_admin',
        N'Platform administrator',
        N'service',
        NULL,
        @CreateDate
    );
END;

GO