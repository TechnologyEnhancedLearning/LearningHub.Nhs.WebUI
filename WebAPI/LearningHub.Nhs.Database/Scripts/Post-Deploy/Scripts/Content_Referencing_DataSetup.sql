-- Content Referenincing data set up script


-- Only run the following code if all PrimaryCatalogueNodeId values are set to 1 in the ResourceVersion table
IF NOT EXISTS (SELECT 1 FROM resources.ResourceVersion WHERE PrimaryCatalogueNodeId > 1)
BEGIN

    -- Update the new PrimaryCatalogueNodeId column in the ResourceVersion table
    DECLARE @ResourceId INT
    DECLARE @ResourceVersionId INT
    DECLARE @ResourceNodeId INT
    DECLARE @PrimaryCatalogueNodeId INT

    DECLARE ResourceVersionCursor CURSOR FOR
    SELECT  Id, ResourceId
    FROM    resources.ResourceVersion rv
    Where rv.Deleted = 0

    OPEN ResourceVersionCursor
    FETCH NEXT FROM ResourceVersionCursor INTO @ResourceVersionId, @ResourceId

    WHILE @@FETCH_STATUS = 0
    BEGIN

        SET @PrimaryCatalogueNodeId = NULL

        SELECT  TOP 1 @PrimaryCatalogueNodeId = np.CatalogueNodeId
        FROM    hierarchy.NodeResource nr
        INNER JOIN  hierarchy.NodePath np ON nr.NodeId = np.NodeId
	    WHERE   nr.ResourceId = @ResourceId
		    AND nr.VersionStatusId = 1 -- Draft
            AND nr.Deleted = 0
            AND np.Deleted = 0
        ORDER BY nr.CreateDate DESC

        IF @PrimaryCatalogueNodeId IS NULL
        BEGIN
                SELECT  TOP 1 @PrimaryCatalogueNodeId = np.CatalogueNodeId
                FROM    hierarchy.NodeResource nr
                INNER JOIN  hierarchy.NodePath np ON nr.NodeId = np.NodeId
		        WHERE   nr.ResourceId = @ResourceId
			        AND nr.VersionStatusId = 2 -- Published
                    AND nr.Deleted = 0
                    AND np.Deleted = 0
                ORDER BY nr.CreateDate DESC
        END

        IF @PrimaryCatalogueNodeId > 1
        BEGIN
		    UPDATE  resources.ResourceVersion
		    SET     PrimaryCatalogueNodeId = @PrimaryCatalogueNodeId
		    WHERE   Id = @ResourceVersionId
	    END

        FETCH NEXT FROM ResourceVersionCursor INTO @ResourceVersionId, @ResourceId
    END

    CLOSE ResourceVersionCursor
    DEALLOCATE ResourceVersionCursor

END
GO


-- Only run the following code if all PrimaryCatalogueNodeId values are set to 1 in the NodeVersion table
IF NOT EXISTS (SELECT 1 FROM hierarchy.NodeVersion WHERE PrimaryCatalogueNodeId > 1)
BEGIN

    -- Update the new PrimaryCatalogueNodeId column in the NodeVersion table
    DECLARE @NodeId INT
    DECLARE @NodeVersionId INT
    DECLARE @PrimaryCatalogueNodeId INT

    DECLARE NodeVersionCursor CURSOR FOR
    SELECT  Id, NodeId
    FROM    hierarchy.NodeVersion nv
    Where nv.Deleted = 0

    OPEN NodeVersionCursor
    FETCH NEXT FROM NodeVersionCursor INTO @NodeVersionId, @NodeId

    WHILE @@FETCH_STATUS = 0
    BEGIN

        SET @PrimaryCatalogueNodeId = NULL

        SELECT  TOP 1 @PrimaryCatalogueNodeId = np.CatalogueNodeId
        FROM    hierarchy.NodePath np
	    WHERE   np.NodeId = @NodeId
            AND np.Deleted = 0
        ORDER BY np.CreateDate DESC

        IF @PrimaryCatalogueNodeId > 1
        BEGIN
		    UPDATE  hierarchy.NodeVersion
		    SET     PrimaryCatalogueNodeId = @PrimaryCatalogueNodeId
		    WHERE   Id = @NodeVersionId
	    END

        FETCH NEXT FROM NodeVersionCursor INTO @NodeVersionId, @NodeId
    END

    CLOSE NodeVersionCursor
    DEALLOCATE NodeVersionCursor

END
GO

IF NOT EXISTS (SELECT 1 FROM  hierarchy.HierarchyEditDetailOperation Where Id =5)
BEGIN
	INSERT INTO [hierarchy].[HierarchyEditDetailOperation]
           ([Id]
           ,[Description]
           ,[Deleted]
           ,[AmendUserId]
           ,[AmendDate]
           ,[CreateUserId]
           ,[CreateDate])
     VALUES
           (5
           ,'Remove Reference'
           ,0
           ,4
           ,SYSDATETIMEOFFSET()
           ,4
           ,SYSDATETIMEOFFSET())
END
GO