-- hierarchy.NodeType
IF NOT EXISTS(SELECT 'X' FROM [hierarchy].[NodeType] WHERE Id = 1)
BEGIN
INSERT INTO [hierarchy].[NodeType] ([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (1, 'Catalogue', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [hierarchy].[NodeType] WHERE Id = 2)
BEGIN
INSERT INTO [hierarchy].[NodeType] ([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (2, 'Course', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
IF NOT EXISTS(SELECT 'X' FROM [hierarchy].[NodeType] WHERE Id = 3)
BEGIN
INSERT INTO [hierarchy].[NodeType] ([Id],[Name],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (3, 'Folder', '', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

-- hierarchy.VersionStatus
IF NOT EXISTS(SELECT 'X' FROM [hierarchy].[VersionStatus] WHERE Id = 1)
BEGIN
	INSERT INTO [hierarchy].[VersionStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (1, 'Draft', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [hierarchy].[VersionStatus] WHERE Id = 2)
BEGIN
	INSERT INTO [hierarchy].[VersionStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (2, 'Published', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
GO