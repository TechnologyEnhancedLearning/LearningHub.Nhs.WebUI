
-------------------------------------------------------------------------------
-- Author       Phil T
-- Created      04-07-24
-- Purpose      Return resources references and catalogues


-- Description
/*

	This procedure returns resources based on either resourceIds or originalResourceIds.
	There is a bool for including external resources with default behaviour being not using them.

	This data is likely to be grouped by resourceId in the application

	resourceIds and originalResourceIds are nullable and most commonly this stored procedure would be used with one or the other.


*/
-- Future Considerations
/*
		Left joining on currentResourceVersion and allowing null on node resource version will allow draft resources to be returned

*/
-- Notes
/*
	Note this stored procedure strips away the catalogue data for nodetype = 4 ExternalSystem
	Which includes the originalResourceId, therefore it is possible to search by originalResourceId and return resourceId data 
	without the originalResourceId used returned within the data itself

	If used with a single originalResourceId only one row should be returned if the data is correct as there should only be one catalogue per originalResourceId.
	If a single resourceId is used this query can return multiple rows.

*/


	
-------------------------------------------------------------------------------



CREATE PROCEDURE [resources].[GetResourceReferencesAndCatalogue]
    @ResourceIds VARCHAR(MAX) = NULL,
    @OriginalResourceIds VARCHAR(MAX) = NULL,
	@includeExternalResources bit = 0 -- default false as our logic generally exclude external resources we do not manage
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @resourceIdTable TABLE (
        resourceId INT
    );
	  DECLARE @originalResourceIdTable TABLE (
        originalResourceId INT
    );

    -- Insert ResourceIds into temporary table
    IF @ResourceIds IS NOT NULL
    BEGIN
        INSERT INTO @resourceIdTable (resourceId)
        SELECT value
        FROM STRING_SPLIT(@ResourceIds, ',');
    END;

    -- Insert OriginalResourceIds into temporary table
    IF @OriginalResourceIds IS NOT NULL
    BEGIN
        INSERT INTO @originalResourceIdTable (originalResourceId)
        SELECT value
        FROM STRING_SPLIT(@OriginalResourceIds, ',');
    END;

select
rref.resourceid as resourceId,
rv.Title as Title,
rv.Description as Description,
rr.ResourceTypeId as ResourceTypeId, -- Becomes ResourceType via ResourceTypeEnum
rv.MajorVersion as MajorVersion,
ISNULL(rvrs.AverageRating, 0) as Rating, -- not all have a rating

-- stripping away data where its nodetype ExternalSystem
    CASE 
        WHEN n.NodeTypeId = 4 THEN NULL 
        ELSE rref.OriginalResourceReferenceId 
    END AS OriginalResourceReferenceId,    -- Note models often store this value as ref.id
    CASE 
        WHEN n.NodeTypeId = 4 THEN NULL 
        ELSE cnnv.NodeId 
    END AS CatalogueNodeId,    -- this corresponds to catalogueNodeVersion.NodeVersion.NodeId
    CASE 
        WHEN n.NodeTypeId = 4 THEN NULL 
        ELSE cn.Name 
    END AS CatalogueNodeName,
    CASE 
        WHEN n.NodeTypeId = 4 THEN NULL 
        ELSE cn.RestrictedAccess 
    END AS IsRestricted


from [learninghub].[resources].[ResourceReference] rref
	join [learninghub].[resources].[Resource] rr on rr.id = rref.resourceid
	join [learninghub].[resources].[ResourceVersion] rv on rv.id = rr.CurrentResourceVersionId
	join [learninghub].[hierarchy].[nodepath] np on np.id = rref.nodepathid -- gets us to catalogue
	join [learninghub].[hierarchy].[node] n on  n.id = np.nodeid
	join [learninghub].[hierarchy].[nodeVersion] nv on  nv.id = n.CurrentNodeVersionId
	join [learninghub].[hierarchy].[CatalogueNodeVersion] cn on cn.NodeVersionId = np.id 
	join [learninghub].[hierarchy].[nodeVersion] cnnv on cnnv.Id = cn.NodeVersionId
	left join [learninghub].[resources].[ResourceVersionRatingSummary] rvrs on rvrs.resourceVersionId = rv.id

where 
(rref.ResourceId IN (SELECT resourceId FROM @resourceIdTable) OR 
 rref.OriginalResourceReferenceId IN (SELECT originalResourceId FROM @originalResourceIdTable))
and rr.deleted = 0
and rv.Deleted = 0
and np.deleted = 0
and n.Deleted = 0
and nv.deleted = 0
and cn.Deleted = 0
and rref.deleted = 0
and cnnv.deleted = 0
and ( rvrs.deleted is null or rvrs.Deleted = 0)
and (n.NodeTypeId != 4 or @includeExternalResources = 1)



End
GO


