BEGIN TRY
    BEGIN TRANSACTION;

	DECLARE @CatalogueId INT = 0; --this is catalog ID in the admin page
	DECLARE @ResourceOwnerId INT = 0; -- this is CreateUserId of the resources to be moved.
	DECLARE @NodePathId INT; 
    DECLARE @NewNodeId INT;
    DECLARE @NextDisplayOrder INT;

	Select @NodePathId = np.Id from hierarchy.CatalogueNodeVersion cnv
	inner join hierarchy.NodeVersion nv ON cnv.NodeVersionId = nv.Id
	inner join hierarchy.Node n ON nv.NodeId = n.Id
	inner join hierarchy.NodePath np ON n.Id = np.NodeId
	Where cnv.Id = @CatalogueId and cnv.Deleted = 0 and nv.Deleted = 0 and n.Deleted = 0 and np.Deleted=0;

    -- Get the NodeId for the new catalogue
    SELECT @NewNodeId = NodeId FROM [hierarchy].[NodePath] WHERE Id = @NodePathId AND Deleted = 0;

    -- Get the current max DisplayOrder for that Node
    SELECT @NextDisplayOrder = ISNULL(MAX(DisplayOrder), 0) FROM [hierarchy].[NodeResource] WHERE NodeId = @NewNodeId AND Deleted = 0;

    DECLARE @UpdatedResources TABLE (ResourceId INT);

    INSERT INTO @UpdatedResources (ResourceId)
    SELECT DISTINCT rr.ResourceId FROM [resources].[ResourceReference] rr
    INNER JOIN [resources].[Resource] r
        ON rr.ResourceId = r.Id
    WHERE r.CreateUserId = @ResourceOwnerId AND r.Deleted = 0 AND rr.CreateUserId = @ResourceOwnerId AND rr.Deleted = 0 AND rr.NodePathId = 1;

    -- Update ResourceReference to point to new catalogue
    UPDATE rr
    SET rr.NodePathId = @NodePathId
    FROM [resources].[ResourceReference] rr
    INNER JOIN @UpdatedResources ur
        ON rr.ResourceId = ur.ResourceId
    WHERE rr.CreateUserId = @ResourceOwnerId
      AND rr.Deleted = 0
      AND rr.NodePathId = 1;

    -- Update NodeResource with new NodeId and incrementing DisplayOrder
    ;WITH Ordered AS
    (
        SELECT 
            nr.ResourceId,
            ROW_NUMBER() OVER (ORDER BY nr.ResourceId) AS RowNum
        FROM [hierarchy].[NodeResource] nr
        INNER JOIN @UpdatedResources ur
            ON nr.ResourceId = ur.ResourceId
        WHERE nr.NodeId = 1
          AND nr.CreateUserId = @ResourceOwnerId
          AND nr.Deleted = 0
    )
    UPDATE nr
    SET nr.NodeId = @NewNodeId,nr.DisplayOrder = @NextDisplayOrder + o.RowNum
    FROM [hierarchy].[NodeResource] nr
    INNER JOIN Ordered o ON nr.ResourceId = o.ResourceId;


    --update noderesourcelookup(Im not sure this is absolutely necessary)
	update nrl set nrl.NodeId=@NewNodeId  FROM [hierarchy].[NodeResourceLookup] nrl
	INNER JOIN @UpdatedResources ur ON nrl.ResourceId = ur.ResourceId
	WHERE nrl.CreateUserId = @ResourceOwnerId AND nrl.Deleted = 0

    -- Show updated resources
    SELECT DISTINCT rv.ResourceId, rv.Title FROM [resources].[ResourceVersion] rv
    INNER JOIN @UpdatedResources ur ON rv.ResourceId = ur.ResourceId;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;

    SELECT 
        ERROR_NUMBER() AS ErrorNumber,
        ERROR_MESSAGE() AS ErrorMessage;
END CATCH;