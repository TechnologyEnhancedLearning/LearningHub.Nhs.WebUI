DECLARE @CurrentMaxId INT
DECLARE @IdIncrement INT
DECLARE @NextId INT

SELECT @CurrentMaxId = MAX(Id) FROM [resources].[FileType]
SET @IdIncrement = 1

IF NOT EXISTS (SELECT 1 FROM [resources].[FileType] WHERE Extension = 'exe')
BEGIN
    SELECT @NextId = @CurrentMaxId + @IdIncrement
    
    INSERT INTO [resources].[FileType] (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (@NextId, 9, 'Executable File','Executable File', 'exe', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())
    
    SELECT @CurrentMaxId = MAX(Id) FROM [resources].[FileType]    
END

IF NOT EXISTS (SELECT 1 FROM [resources].[FileType] WHERE Extension = 'dll')
BEGIN
    SELECT @NextId = @CurrentMaxId + @IdIncrement

    INSERT INTO [resources].[FileType] (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (@NextId, 9, 'Dynamic-Link Library File','Dynamic-Link Library File', 'dll', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())

    SELECT @CurrentMaxId = MAX(Id) FROM [resources].[FileType]
END

IF NOT EXISTS (SELECT 1 FROM [resources].[FileType] WHERE Extension = 'app')
BEGIN
    SELECT @NextId = @CurrentMaxId + @IdIncrement

    INSERT INTO [resources].[FileType] (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (@NextId, 9, 'Apple Mac OS X Application File','Apple Mac OS X Application File', 'app', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())

    SELECT @CurrentMaxId = MAX(Id) FROM [resources].[FileType]
END

IF NOT EXISTS (SELECT 1 FROM [resources].[FileType] WHERE Extension = 'dmg')
BEGIN
    SELECT @NextId = @CurrentMaxId + @IdIncrement

    INSERT INTO [resources].[FileType] (Id, DefaultResourceTypeId, Name, Description, Extension, NotAllowed, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (@NextId, 9, 'Apple Mac OS X Disk Image File','Apple Mac OS X Disk Image File', 'dmg', 1, 0, 0, SYSDATETIMEOFFSET(), 0, SYSDATETIMEOFFSET())

    SELECT @CurrentMaxId = MAX(Id) FROM [resources].[FileType]
END
