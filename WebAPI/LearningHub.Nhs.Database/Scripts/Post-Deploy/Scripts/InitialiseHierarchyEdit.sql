/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

UPDATE [resources].[ResourceReference] SET OriginalResourceReferenceId=Id

-- Update existing Catalogue data:
--	Create NodePathNode and NodeResourceLookup entries
DECLARE @AmendUserId int = 4
DECLARE @AmendDate datetimeoffset(7) = SYSDATETIMEOFFSET()
DECLARE @NodeId int
DECLARE @CatalogueCursor as CURSOR
 
SET @CatalogueCursor = CURSOR FORWARD_ONLY FOR
SELECT DISTINCT nv.NodeId
FROM hierarchy.CatalogueNodeVersion cnv
JOIN hierarchy.NodeVersion nv ON cnv.NodeVersionId = nv.Id
WHERE cnv.Deleted = 0

OPEN @CatalogueCursor;
FETCH NEXT FROM @CatalogueCursor INTO @NodeId;
	WHILE @@FETCH_STATUS = 0
BEGIN
	
	----------------------------------------------------------
	-- NodePathNode
	----------------------------------------------------------		
	-- Generate NodePathNode entries
	DECLARE @Id int
	DECLARE @NodePath as NVARCHAR(256)

	SELECT @Id = Id, @NodePath = NodePath 
	FROM hierarchy.NodePath 
	WHERE NodeId = @NodeId and CatalogueNodeId = @NodeId AND Deleted = 0 

	IF NOT EXISTS (SELECT 'x' FROM [hierarchy].[NodePathNode] WHERE NodeId = @NodeId)
	BEGIN

		INSERT INTO [hierarchy].[NodePathNode]([NodePathId],[NodeId],[Depth],Deleted,[CreateUserID],[CreateDate],[AmendUserID],[AmendDate])
		SELECT
			NodePathId = @Id,
			NodeId = nodeInPath.[Value],
			Depth = nodeInPath.idx,
			Deleted = 0,
			CreateUserID = @AmendUserId,
			CreateDate = @AmendDate,
			AmendUserID = @AmendUserId,
			AmendDate = @AmendDate
		FROM
			hub.[fn_Split](@NodePath, '\') nodeInPath
	END

	----------------------------------------------------------
	-- NodeResourceLookup
	----------------------------------------------------------
	IF NOT EXISTS (SELECT 'x' FROM [hierarchy].NodeResourceLookup WHERE NodeId = @NodeId)
	BEGIN

		INSERT INTO hierarchy.NodeResourceLookup ([NodeId],[ResourceId],[Deleted],[CreateUserID],[CreateDate],[AmendUserID],[AmendDate])
		SELECT nr.NodeId, nr.ResourceId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
		FROM
			hierarchy.[NodePath] np
		INNER JOIN
			hierarchy.NodeResource nr ON np.NodeId = nr.NodeId
		LEFT JOIN 
			hierarchy.NodeResourceLookup nrl ON nr.NodeId = nrl.NodeId AND nr.ResourceId = nrl.ResourceId
		WHERE
			np.NodeId = @NodeId and np.CatalogueNodeId = @NodeId AND np.Deleted = 0 AND nrl.Id IS NULL
	
	END

	FETCH NEXT FROM @CatalogueCursor INTO @NodeId;

END

CLOSE @CatalogueCursor;
DEALLOCATE @CatalogueCursor;


-- [hierarchy].[HierarchyEditStatus]
IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditStatus] WHERE Id = 1)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (1, 'Draft', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset())
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditStatus] WHERE Id = 2)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (2, 'Published', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditStatus] WHERE Id = 3)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (3, 'Discarded', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditStatus] WHERE Id = 4)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (4, 'Publishing', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditStatus] WHERE Id = 5)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (5, 'Submitted to Publishing Queue', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditStatus] WHERE Id = 6)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditStatus] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (6, 'Failed to Publish', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

-- [hierarchy].[HierarchyEditDetailType]
IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailType] WHERE Id = 1)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditDetailType] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (1, 'Catalogue Node', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailType] WHERE Id = 2)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditDetailType] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (2, 'Course Node', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailType] WHERE Id = 3)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditDetailType] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (3, 'Folder Node', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset())
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailType] WHERE Id = 4)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditDetailType] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (4, 'Node Link', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset())
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailType] WHERE Id = 5)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditDetailType] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (5, 'Node Resource', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset())
END
GO

-- [hierarchy].[HierarchyEditDetailOperation]
IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailOperation] WHERE Id = 1)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditDetailOperation] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
		 VALUES (1, 'Add', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailOperation] WHERE Id = 2)
BEGIN
INSERT INTO [hierarchy].[HierarchyEditDetailOperation] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (2, 'Edit', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END

GO
IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailOperation] WHERE Id = 3)
BEGIN
INSERT INTO [hierarchy].[HierarchyEditDetailOperation] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
     VALUES (3, 'Delete', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

-- [hub].[EventLog]
IF NOT EXISTS (SELECT 1 FROM [hub].[EventLog] WHERE Id = 1)
BEGIN
INSERT INTO [hub].[EventLog] ([Id],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (1, 'Content Structure Admin', 'Content Structure Admin', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

-- [hub].[EventType]
IF NOT EXISTS (SELECT 1 FROM [hub].[EventType] WHERE Id = 1)
BEGIN
INSERT INTO [hub].[EventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (1, 'Critical', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hub].[EventType] WHERE Id = 2)
BEGIN
INSERT INTO [hub].[EventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (2, 'Error', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hub].[EventType] WHERE Id = 3)
BEGIN
INSERT INTO [hub].[EventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (3, 'Information', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO

IF NOT EXISTS (SELECT 1 FROM [hub].[EventType] WHERE Id = 4)
BEGIN
INSERT INTO [hub].[EventType] ([Id],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (4, 'Warning', 0, 4, sysdatetimeoffset(), 4, sysdatetimeoffset()) 
END
GO


-----------------------------------------------------------
-- NodeResource.DisplayOrder - Backfill of existing records
-----------------------------------------------------------
-- Those catalogues whose default sort order is by date (desc)
;WITH cte AS 
(
	SELECT nr.DisplayOrder, RowNum = ROW_NUMBER() OVER (PARTITION BY n.Id ORDER BY rv.CreateDate DESC)
	FROM hierarchy.NodeResource nr
		INNER JOIN hierarchy.Node n ON nr.NodeId = n.Id
		INNER JOIN hierarchy.NodePath np ON np.NodeId = n.Id
		INNER JOIN hierarchy.Node nc ON np.CatalogueNodeId = nc.Id
		INNER JOIN hierarchy.NodeVersion nvc ON nvc.Id = nc.CurrentNodeVersionId
		INNER JOIN hierarchy.CatalogueNodeVersion cnv ON nvc.Id = cnv.NodeVersionId
		INNER JOIN resources.Resource r ON r.Id = nr.ResourceId
		INNER JOIN resources.ResourceVersion rv ON rv.ResourceId = r.Id
	WHERE nr.Deleted = 0 AND n.Deleted = 0 AND np.Deleted = 0 AND nvc.Deleted = 0 AND cnv.Deleted = 0
		AND cnv.[Order] = 1 AND nr.DisplayOrder IS NULL
		AND (r.CurrentResourceVersionId = rv.Id OR (r.CurrentResourceVersionId  IS NULL AND (rv.VersionStatusId = 1 OR rv.VersionStatusId = 4 OR rv.VersionStatusId = 5 OR rv.VersionStatusId = 6)))
		AND n.Id <> 1
)
UPDATE cte
SET DisplayOrder = RowNum
-- Those catalogues whose default sort order is alphabetical
;WITH cte AS
(
	SELECT nr.DisplayOrder, RowNum = ROW_NUMBER() OVER (PARTITION BY n.Id ORDER BY rv.Title)
	FROM hierarchy.NodeResource nr
		INNER JOIN hierarchy.Node n ON nr.NodeId = n.Id
		INNER JOIN hierarchy.NodePath np ON np.NodeId = n.Id
		INNER JOIN hierarchy.Node nc ON np.CatalogueNodeId = nc.Id
		INNER JOIN hierarchy.NodeVersion nvc ON nvc.Id = nc.CurrentNodeVersionId
		INNER JOIN hierarchy.CatalogueNodeVersion cnv ON nvc.Id = cnv.NodeVersionId
		INNER JOIN resources.Resource r ON r.Id = nr.ResourceId
		INNER JOIN resources.ResourceVersion rv ON rv.ResourceId = r.Id
	WHERE nr.Deleted = 0 AND n.Deleted = 0 AND np.Deleted = 0 AND nvc.Deleted = 0 AND cnv.Deleted = 0
		AND cnv.[Order] = 0 AND nr.DisplayOrder IS NULL
		AND (r.CurrentResourceVersionId = rv.Id OR (r.CurrentResourceVersionId  IS NULL AND (rv.VersionStatusId = 1 OR rv.VersionStatusId = 4 OR rv.VersionStatusId = 5 OR rv.VersionStatusId = 6)))
		AND n.Id <> 1
)
UPDATE cte
SET DisplayOrder = RowNum