-------------------------------------------------------------------------------
-- Author       DJB
-- Created      11 Oct 2023
-- Purpose      Gets the generic file content details
--
-- Modification History
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetGenericFileContentDetails]
	@resourceVersionId		INT,
	@UserId					INT
AS
BEGIN

		DECLARE @IsEditor BIT
		SET		@IsEditor = resources.UserCanEditResource(@resourceVersionId, @UserId)

		--SELECT @IsEditor = CASE WHEN nr.NodeId IS NULL THEN CAST (0 AS BIT) ELSE CAST(1 AS BIT) END 
		--FROM resources.Resource r 
		--INNER JOIN hierarchy.NodeResource nr ON nr.ResourceId = r.Id AND nr.VersionStatusId = 2 -- Published
		--INNER JOIN hub.Scope s ON nr.NodeId = s.CatalogueNodeId
		--INNER JOIN hub.RoleUserGroup rug ON rug.ScopeId = s.Id AND rug.RoleId = 1 -- Editor
		--INNER JOIN hub.userUserGroup uug ON uug.UserGroupId = rug.UserGroupId AND uug.UserId = @userId
		--WHERE	r.CurrentResourceVersionId = @resourceVersionId
		--	AND nr.Deleted = 0 
		--	AND s.Deleted = 0
		--	AND rug.Deleted = 0
		--	AND uug.Deleted = 0


		DECLARE @NodePathId INT
				
		SELECT @NodePathId = np.Id
		FROM hierarchy.NodePath np
		INNER JOIN hierarchy.Node n ON np.NodeId = n.Id 
		WHERE	n.NodeTypeId = 4 -- External Organisation

		SELECT
		ISNULL(er.Id, 0) AS ExternalReferenceId,   
        gfrv.EsrLinkTypeId EsrLinkType
        ,CASE WHEN gfrv.CreateUserId = @UserId THEN CAST(1 AS BIT) ELSE ISNULL(@IsEditor, CAST(0 AS BIT)) END  IsOwnerOrEditor
        ,er.ExternalReference ExternalReference       
        ,'' HostedContentUrl
		,ur.FullHistoricUrl
		FROM resources.ResourceVersion rv
		JOIN resources.GenericFileResourceVersion gfrv ON gfrv.ResourceVersionId = rv.Id
		LEFT OUTER JOIN resources.ResourceReference rr ON rr.ResourceId = rv.ResourceId AND rr.NodePathId = @NodePathId
		LEFT OUTER JOIN resources.ExternalReference er On er.ResourceReferenceId = rr.Id
		LEFT OUTER JOIN resources.UrlRewriting ur on ur.ExternalReferenceId = er.Id
		WHERE rv.id = @resourceVersionId
END