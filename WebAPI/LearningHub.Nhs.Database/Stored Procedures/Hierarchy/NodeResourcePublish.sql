-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Publishes a NodeResource.
--
-- Modification History
--
-- 08-10-2021  KD	Initial Revision. Provides functionality for IT1.
--					Note: IT1 assumes a Resource exists in a single Node location.
-- 21-12-2021  RS   Fix to NodeResource update when republishing unpublished resource.
-- 09-02-2022  KD	Explicitly exclude External Orgs from Resource Reference processing.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[NodeResourcePublish]
(
	@NodeId int,
	@ResourceId int,
	@PublicationId int,
	@AmendUserId int
)
AS

BEGIN

	DECLARE @AmendDate datetimeoffset(7) = SYSDATETIMEOFFSET()
	
	-- Ensure an Active NodePath
	-- IT1 - NodePath can be obtained from the NodeId
	-- IT2 - NodePath to be supplied as param
	DECLARE @NodePathId int

	SELECT	@NodePathId = Id
	FROM	hierarchy.NodePath
	WHERE	NodeId = @NodeId AND Deleted = 0 AND IsActive = 1

	IF @NodePathId IS NULL
	BEGIN
		RAISERROR ('NodeResourcePublish: Error - An active NodePath is required', -- Message text.  
					16, -- Severity.  
					1 -- State.  
					);  
	END

	-- If not part of an existing ResourceVersion publication then create a new Publication record.
	IF @PublicationId IS NULL
	BEGIN
		DECLARE @ResourceVersionId int
		SELECT @ResourceVersionId = CurrentResourceVersionId FROM resources.[Resource] WHERE Id=@ResourceId AND Deleted=0

		IF @ResourceVersionId IS NULL
		BEGIN
			RAISERROR ('NodeResourcePublish: Error - A published ResourceVersion is required', -- Message text.  
					   16, -- Severity.  
					   1 -- State.  
					   );  
		END

		DECLARE @NodeVersionId int
		SELECT @NodeVersionId = CurrentNodeVersionId FROM hierarchy.[Node] WHERE Id=@NodeId

		INSERT INTO [hierarchy].[Publication] ([ResourceVersionId],[NodeVersionId],[Notes],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (@ResourceVersionId, @NodeVersionId, '', 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate)

		SELECT @PublicationId = SCOPE_IDENTITY();
	END

	---- IT1 - resource may only be published in one location
	DECLARE @PublishedSourceNodeId INT, @PublishedSourceDisplayOrder INT
	SELECT @PublishedSourceNodeId = NodeId, @PublishedSourceDisplayOrder = DisplayOrder 
		FROM hierarchy.NodeResource WHERE ResourceId = @ResourceId AND VersionStatusId = 2 AND Deleted = 0

	UPDATE	nr
	SET 			
		Deleted		=	1,
		AmendDate	=	@AmendDate,
		AmendUserId	=	@AmendUserId
	FROM	hierarchy.NodeResource nr
	INNER JOIN	hierarchy.Node n ON nr.NodeId = n.Id
	WHERE nr.ResourceId = @ResourceId
		AND	nr.VersionStatusId = 2
		AND	nr.Deleted = 0

	-- Update the DisplayOrder of resources that appeared after this one in the source node - decrease by 1.
	IF @PublishedSourceNodeId > 1
	BEGIN
		UPDATE [hierarchy].[NodeResource] SET DisplayOrder = DisplayOrder - 1, AmendDate = @AmendDate, AmendUserId = @AmendUserId 
		WHERE NodeId = @PublishedSourceNodeId AND Deleted = 0 AND DisplayOrder > @PublishedSourceDisplayOrder
	END

	-- Ensure Resource is published under the supplied NodeId
	IF EXISTS (
				SELECT	1
				FROM		hierarchy.NodeResource nr
				INNER JOIN	hierarchy.Node n ON nr.NodeId = n.Id
				WHERE	nr.ResourceId = @ResourceId
					AND nr.NodeId = @NodeId
					AND nr.VersionStatusId IN (1, 3)
					AND nr.Deleted = 0
				)
	BEGIN
		-- IT1 - update existing NodeResource.
		UPDATE	nr
		SET 			
			VersionStatusId	=	2,
			PublicationId	=	@PublicationId,
			AmendDate		=	@AmendDate,
			AmendUserId		=	@AmendUserId
		FROM	hierarchy.NodeResource nr WITH (NOLOCK)
		INNER JOIN	hierarchy.Node n WITH (NOLOCK) ON nr.NodeId = n.Id
		WHERE nr.ResourceId = @ResourceId
			AND nr.NodeId = @NodeId
			AND nr.VersionStatusId IN (1, 3)
			AND	nr.Deleted = 0

	END
	ELSE
	BEGIN

		-- IT1 - new NodeResource.
		INSERT INTO [hierarchy].[NodeResource] ([NodeId],[ResourceId],[DisplayOrder],[VersionStatusId],[PublicationId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (
			@NodeId, 
			@ResourceId, 
			CASE @NodeId
				WHEN 1 THEN NULL
				ELSE 1
			END, 
			2, 
			@PublicationId, 
			0, 
			@AmendUserId, 
			@AmendDate, 
			@AmendUserId, 
			@AmendDate)

		 --Increment DisplayOrder on existing NodeResources if node is not the Community Contributions catalogue.
		IF @NodeId > 1
		BEGIN
			UPDATE [hierarchy].[NodeResource] SET DisplayOrder = DisplayOrder + 1, AmendDate = @AmendDate, AmendUserId = @AmendUserId WHERE NodeId = @NodeId AND Deleted = 0 AND ResourceId != @ResourceId
			UPDATE [hierarchy].NodeLink SET DisplayOrder = DisplayOrder + 1, AmendDate = @AmendDate, AmendUserId = @AmendUserId WHERE ParentNodeId = @NodeId AND Deleted = 0 
		END

	END

	----------------------------------------------------------
	-- ResourceReference
	----------------------------------------------------------
	-- IT1/2 - A resource can only exist in one location
	-- Note: future iterations may require refactoring required to allow a Resource to be referenced in multiple paths.
	DECLARE @ExistingResourceReferenceId int
	DECLARE @OriginalResourceReferenceId int
	SELECT 
		@ExistingResourceReferenceId = rr.Id, 
		@OriginalResourceReferenceId = rr.OriginalResourceReferenceId 
	FROM 
		resources.ResourceReference rr
	INNER JOIN
		hierarchy.NodePath np ON rr.NodePathId = np.Id
	INNER JOIN 
		hierarchy.[Node] n ON np.NodeId = n.Id
	WHERE 
		ResourceId = @ResourceId
		AND n.NodeTypeId IN (1, 2, 3) -- Catalogue, Course, Folder only
		AND rr.NodePathId NOT IN (SELECT np.Id FROM hierarchy.NodePath np INNER JOIN [hub].[ExternalOrganisation] eo ON eo.NodeId = np.NodeId) -- explicitly exclude Ext Refs
		AND rr.Deleted = 0
		AND np.Deleted = 0
		AND n.Deleted = 0

	IF @OriginalResourceReferenceId IS NULL
	BEGIN
		INSERT INTO	resources.ResourceReference (ResourceId, NodePathId, OriginalResourceReferenceId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		VALUES (@ResourceId, @NodePathId, NULL, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate)

		UPDATE rr
		SET OriginalResourceReferenceId = Id 
		FROM resources.ResourceReference rr 
		WHERE Id = SCOPE_IDENTITY() AND OriginalResourceReferenceId IS NULL
	END
	ELSE
	BEGIN
		INSERT INTO	resources.ResourceReference (ResourceId, NodePathId, OriginalResourceReferenceId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		VALUES (@ResourceId, @NodePathId, @OriginalResourceReferenceId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate)

		UPDATE resources.ResourceReference
		SET Deleted = 1 
		WHERE Id = @ExistingResourceReferenceId
	END

	----------------------------------------------------------
	-- NodeResourceLookup
	----------------------------------------------------------
	-- IT1 - new table to provide quick lookup all Published Resources under a node
	;WITH cteNodeResource(NodeId, ParentNodeId, ResourceId)
	AS (
		SELECT 
			nr.NodeId,
			nl.ParentNodeId,
			nr.ResourceId
		FROM
			[hierarchy].[NodeResource] nr
		LEFT JOIN
			hierarchy.NodeLink nl ON nr.NodeId = nl.ChildNodeId AND nl.Deleted = 0
		WHERE
			nr.NodeId = @NodeId 
			AND nr.ResourceId = @ResourceId
			AND nr.VersionStatusId = 2 -- Published
			AND nr.Deleted = 0
		
		UNION ALL

		SELECT 
			cte.ParentNodeId AS NodeId,
			nl.ParentNodeId AS ParentNodeId,
			cte.ResourceId
		FROM
			cteNodeResource cte			
		INNER JOIN
			hierarchy.NodeLink nl ON nl.ChildNodeId = cte.ParentNodeId
		WHERE
			nl.Deleted = 0
			AND nl.Deleted = 0
	  )
	SELECT NodeId, ParentNodeId, ResourceId
	INTO #cteNodeResource
	FROM cteNodeResource

	-- Add "root node" resource lookup info
	INSERT INTO #cteNodeResource
	SELECT nr1.ParentNodeId AS NodeId, NULL AS ParentNodeId, nr1.ResourceId
	FROM #cteNodeResource nr1
	LEFT JOIN #cteNodeResource nr2 ON nr1.ParentNodeId = nr2.NodeId
	WHERE nr2.NodeId IS NULL AND nr1.ParentNodeId IS NOT NULL

	-- Inserts
	INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
    SELECT DISTINCT @PublicationId, cte.NodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
	FROM #cteNodeResource cte
	LEFT JOIN hierarchy.NodeResourceLookup nrl ON cte.NodeId = nrl.NodeId AND cte.ResourceId = nrl.ResourceId
	WHERE nrl.Id IS NULL

	INSERT INTO hierarchy.NodeResourceLookup ([NodeId],[ResourceId],[Deleted],[CreateUserID],[CreateDate],[AmendUserID],[AmendDate])
    SELECT cte.NodeId, cte.ResourceId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
	FROM #cteNodeResource cte
	LEFT JOIN hierarchy.NodeResourceLookup nrl ON cte.NodeId = nrl.NodeId AND cte.ResourceId = nrl.ResourceId
	WHERE nrl.Id IS NULL
	
	-- Reinstate/s
	INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
    SELECT DISTINCT @PublicationId, nrl.NodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
	FROM hierarchy.NodeResourceLookup nrl
	INNER JOIN #cteNodeResource cte ON cte.NodeId = nrl.NodeId AND cte.ResourceId = nrl.ResourceId AND nrl.deleted = 1

	UPDATE nrl
	SET deleted = 0, AmendUserId = @AmendUserId, AmendDate = @AmendDate
	FROM hierarchy.NodeResourceLookup nrl
	INNER JOIN #cteNodeResource cte ON cte.NodeId = nrl.NodeId AND cte.ResourceId = nrl.ResourceId AND nrl.deleted = 1
	
	-- Delete/s
	INSERT INTO [hierarchy].[PublicationLog] ([PublicationId],[NodeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
    SELECT DISTINCT @PublicationId, nrl.NodeId, 0, @AmendUserId, @AmendDate, @AmendUserId, @AmendDate
	FROM hierarchy.NodeResourceLookup nrl
	INNER JOIN (SELECT DISTINCT ResourceId FROM #cteNodeResource) cte1 ON cte1.ResourceId = nrl.ResourceId
	LEFT JOIN #cteNodeResource cte2 ON cte2.NodeId = nrl.NodeId AND cte2.ResourceId = nrl.ResourceId
	WHERE cte2.NodeId IS NULL AND nrl.Deleted = 0

	UPDATE nrl
	SET deleted = 1, AmendUserId = @AmendUserId, AmendDate = @AmendDate
	FROM hierarchy.NodeResourceLookup nrl
	INNER JOIN (SELECT DISTINCT ResourceId FROM #cteNodeResource) cte1 ON cte1.ResourceId = nrl.ResourceId
	LEFT JOIN #cteNodeResource cte2 ON cte2.NodeId = nrl.NodeId AND cte2.ResourceId = nrl.ResourceId
	WHERE cte2.NodeId IS NULL AND nrl.Deleted = 0

END
GO