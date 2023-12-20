/*
	KD - Set up data prior to release of the Restricted Catalogue Feature
*/
IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceVersionEventType]  WHERE Id = 6)
BEGIN
	INSERT INTO [resources].[ResourceVersionEventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES(6, 'Unpublished by Admin', 0, 1, SYSDATETIMEOFFSET(), 1, SYSDATETIMEOFFSET())
END
GO
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

IF NOT EXISTS (SELECT 1 FROM [hub].[ScopeType] WHERE id = 1)
BEGIN
	INSERT INTO [hub].[ScopeType]([Id],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES (1, 'Catalogue', 'Catalogue Scope Type', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
END
GO

SET IDENTITY_INSERT [hub].[Role] ON

IF NOT EXISTS (SELECT 1 FROM [hub].[Role] WHERE id = 1)
BEGIN
	INSERT INTO [hub].[Role] ([Id],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES (1,'Editor','Editor role',0, 4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS (SELECT 1 FROM [hub].[Role] WHERE id = 2)
BEGIN
	INSERT INTO [hub].[Role] ([Id],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES (2,'Reader','Reader role',0, 4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END

IF NOT EXISTS (SELECT 1 FROM [hub].[Role] WHERE id = 3)
BEGIN
	INSERT INTO [hub].[Role] ([Id],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		 VALUES (3,'Local Admin','Local Admin role',0, 4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET())
END

SET IDENTITY_INSERT [hub].[Role] OFF

IF NOT EXISTS(SELECT Id FROM [hub].[NotificationType] WHERE [Name] = 'AccessRequest')
BEGIN
	SET IDENTITY_INSERT [hub].[NotificationType] ON

	INSERT INTO [hub].[NotificationType] 
		([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
	VALUES 
		(8, 'AccessRequest', 'Access request', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

	SET IDENTITY_INSERT [hub].[NotificationType] OFF
END


-- Migrate data from hierarchy.NodeEditor to Auth schema
IF (EXISTS (SELECT * 
            FROM INFORMATION_SCHEMA.TABLES 
            WHERE TABLE_SCHEMA = 'hierarchy' 
            AND  TABLE_NAME = 'NodeEditor'))
BEGIN

	DECLARE @UserId int = 4
	DECLARE @NodeId int
	DECLARE @NodeEditorCursor as CURSOR
 
	SET @NodeEditorCursor = CURSOR FORWARD_ONLY FOR
	SELECT DISTINCT
		NodeId
	FROM
		hierarchy.NodeEditor ne
	WHERE
		Deleted = 0

	OPEN @NodeEditorCursor;
	FETCH NEXT FROM @NodeEditorCursor INTO @NodeId;
		WHILE @@FETCH_STATUS = 0
	BEGIN

		DECLARE @ScopeId int = NULL
		DECLARE @EditorUserGroupId int = NULL

		SELECT @ScopeId = Id FROM hub.[Scope] WHERE CatalogueNodeId = @NodeId AND Deleted = 0
	
		IF @ScopeId IS NULL
		BEGIN
			INSERT INTO [hub].[Scope] ([ScopeTypeId],[CatalogueNodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
			SELECT 
				1 AS ScopeTypeId, -- Catalogue Scope Type
				@NodeId AS CatalogueNodeId,
				0 as Deleted,
				@UserId as CreateUserId,
				SYSDATETIMEOFFSET() as CreateDate,
				@UserId as AmendUserId,
				SYSDATETIMEOFFSET() as AmendDate
			SELECT @ScopeId = SCOPE_IDENTITY()
		END

		DECLARE @CatalogueName nvarchar(255)
		SELECT 
			@CatalogueName = cnv.[Name]
		FROM
			[hierarchy].[Node] n
			INNER JOIN [hierarchy].[NodeVersion] nv ON n.CurrentNodeVersionId = nv.Id
			INNER JOIN [hierarchy].[CatalogueNodeVersion] cnv ON nv.Id = cnv.NodeVersionId
		WHERE
			n.Id = @NodeId
			AND n.Deleted = 0
			AND nv.Deleted = 0
			AND cnv.Deleted = 0

		SELECT 'CatalogueName: ' + @CatalogueName
		SELECT @EditorUserGroupId = Id FROM [hub].[UserGroup] WHERE [Name] = @CatalogueName + ' - Editors'

		IF @EditorUserGroupId IS NULL 
		BEGIN
			INSERT INTO [hub].[UserGroup]([Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
			SELECT 
				@CatalogueName + ' - Editors' AS [Name],
				'Editors of ' + @CatalogueName +' Catalogue' AS [Description],
				0 as Deleted,
				@UserId as CreateUserId,
				SYSDATETIMEOFFSET() as CreateDate,
				@UserId as AmendUserId,
				SYSDATETIMEOFFSET() as AmendDate
			SELECT @EditorUserGroupId = SCOPE_IDENTITY()
		END
	
		PRINT @CatalogueName + ': Editor UserGroupId = ' + CAST(@EditorUserGroupId AS nvarchar(12))

		DECLARE @RoleUserGroupId int = NULL
		SELECT @RoleUserGroupId = Id FROM hub.RoleUserGroup WHERE UserGroupId = @EditorUserGroupId AND RoleId = 1 AND ScopeId = @ScopeId AND Deleted = 0

		IF @RoleUserGroupId IS NULL
		BEGIN
			INSERT INTO [hub].[RoleUserGroup] ([RoleId],[UserGroupId],[ScopeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
			SELECT 
				1 AS [RoleId], -- Editor
				@EditorUserGroupId AS [UserGroupId], 
				@ScopeId AS [ScopeId],
				0 as Deleted,
				@UserId as CreateUserId,
				SYSDATETIMEOFFSET() as CreateDate,
				@UserId as AmendUserId,
				SYSDATETIMEOFFSET() as AmendDate
		END

		INSERT INTO [hub].[UserUserGroup] ([UserId],[UserGroupId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT
			ne.EditorUserId AS UserId,
			@EditorUserGroupId AS [UserGroupId], 
			0 as Deleted,
			@UserId as CreateUserId,
			SYSDATETIMEOFFSET() as CreateDate,
			@UserId as AmendUserId,
			SYSDATETIMEOFFSET() as AmendDate
		FROM 
			hierarchy.NodeEditor ne
		LEFT JOIN
			[hub].[UserUserGroup] uug ON uug.UserId = ne.EditorUserId AND uug.UserGroupId = @EditorUserGroupId AND uug.Deleted = 0
		WHERE 
			NodeId = @NodeId AND ne.Deleted = 0
			AND uug.Id IS NULL

		FETCH NEXT FROM @NodeEditorCursor INTO @NodeId;
	END

	CLOSE @NodeEditorCursor;
	DEALLOCATE @NodeEditorCursor;


	-- DROP NodeEditor table
	DROP TABLE hierarchy.NodeEditor

END