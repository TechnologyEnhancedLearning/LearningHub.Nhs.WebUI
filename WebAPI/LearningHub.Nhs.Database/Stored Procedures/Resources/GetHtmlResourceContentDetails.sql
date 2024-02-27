-------------------------------------------------------------------------------
-- Author       DV
-- Created      13 Oct 2023
-- Purpose      Gets the html resource content details
--
-- Modification History
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetHtmlResourceContentDetails]
	@resourceVersionId		INT,
	@UserId					INT
AS
BEGIN

		DECLARE @IsEditor BIT
		SET		@IsEditor = 0

		SET		@IsEditor = resources.UserCanEditResource(@resourceVersionId, @UserId)
		
		DECLARE @NodePathId INT
				
		SELECT @NodePathId = np.Id
		FROM hierarchy.NodePath np
		INNER JOIN hierarchy.Node n ON np.NodeId = n.Id 
		WHERE	n.NodeTypeId = 4 -- External Organisation

		SELECT
		ISNULL(er.Id, 0) AS ExternalReferenceId,   
        hrv.EsrLinkTypeId EsrLinkType
        ,CASE WHEN hrv.CreateUserId = @UserId THEN CAST(1 AS BIT) ELSE ISNULL(@IsEditor, CAST(0 AS BIT)) END  IsOwnerOrEditor
        ,er.ExternalReference ExternalReference
        ,'' HostedContentUrl
		,ur.FullHistoricUrl
		FROM resources.ResourceVersion rv
		JOIN resources.HtmlResourceVersion hrv ON hrv.ResourceVersionId = rv.Id
		LEFT OUTER JOIN resources.ResourceReference rr ON rr.ResourceId = rv.ResourceId AND rr.NodePathId = @NodePathId
		LEFT OUTER JOIN resources.ExternalReference er On er.ResourceReferenceId = rr.Id
		LEFT OUTER JOIN resources.UrlRewriting ur on ur.ExternalReferenceId = er.Id
		WHERE rv.id = @resourceVersionId
END