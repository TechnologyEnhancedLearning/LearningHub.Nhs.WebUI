-------------------------------------------------------------------------------
-- Author       HV
-- Created      21 MAR 2021
-- Purpose      Gets the scorm content details
--
-- Modification History
-- 26 May 2021	DJB	@IsEditor defaulted to false before check
--				ExternalReference made optional
-- 05 Aug 2021  RS @IsEditor changed to use the userGroup/role/scope tables.
-- 10 Oct 2023	IsEditor logic moved to a function to allow reuse.
-- 24 Oct 2023	Renamed from GetScormContentDetails to GetExternalContentDetails
--				Updated for multiple resource types
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetExternalContentDetails]
	@resourceVersionId		INT,
	@UserId					INT
AS
BEGIN

		DECLARE @IsEditor BIT
		SET		@IsEditor = resources.UserCanEditResource(@resourceVersionId, @UserId)

		DECLARE @NodePathId INT
				
		SELECT @NodePathId = np.Id
		FROM hierarchy.NodePath np
		INNER JOIN hierarchy.Node n ON np.NodeId = n.Id 
		WHERE	n.NodeTypeId = 4 -- External Organisation

		DECLARE @ResourceTypeId			INT
		SELECT	@ResourceTypeId = ResourceTypeId
		FROM	resources.Resource r
		INNER JOIN	resources.ResourceVersion rv on r.Id = rv.ResourceId
		WHERE	rv.Id = @resourceVersionId

		IF @ResourceTypeId = 6 -- SCORM e-learning resource
		BEGIN
			SELECT
					ISNULL(er.Id, 0) AS ExternalReferenceId,   
					srv.EsrLinkTypeId EsrLinkType,
					CASE WHEN srv.CreateUserId = @UserId
							THEN CAST(1 AS BIT) 
							ELSE ISNULL(@IsEditor, CAST(0 AS BIT))
					END  IsOwnerOrEditor,
					srv.ContentFilePath,
					er.ExternalReference ExternalReference,
					srvm.ManifestUrl DefaultUrl,
					'' HostedContentUrl,
					ur.FullHistoricUrl
					--,srv.ClearSuspendData
			FROM resources.ResourceVersion rv
			JOIN resources.ScormResourceVersion srv ON srv.ResourceVersionId = rv.Id
			JOIN resources.ScormResourceVersionManifest srvm ON srvm.ScormResourceVersionId = srv.Id
			LEFT OUTER JOIN resources.ResourceReference rr ON rr.ResourceId = rv.ResourceId AND rr.NodePathId = @NodePathId
			LEFT OUTER JOIN resources.ExternalReference er On er.ResourceReferenceId = rr.Id
			LEFT OUTER JOIN resources.UrlRewriting ur on ur.ExternalReferenceId = er.Id
			WHERE rv.id = @resourceVersionId
		END
		ELSE IF @ResourceTypeId = 9 -- Generic File resource
		BEGIN
			SELECT
					ISNULL(er.Id, 0) AS ExternalReferenceId,   
					gfrv.EsrLinkTypeId EsrLinkType,
					CASE WHEN gfrv.CreateUserId = @UserId
							THEN CAST(1 AS BIT) 
							ELSE ISNULL(@IsEditor, CAST(0 AS BIT))
					END  IsOwnerOrEditor,
					f.FilePath ContentFilePath,
					er.ExternalReference ExternalReference,
					f.[FileName] DefaultUrl,
					'' HostedContentUrl,
					ur.FullHistoricUrl
					--,srv.ClearSuspendData
			FROM resources.ResourceVersion rv
			INNER JOIN	resources.GenericFileResourceVersion gfrv ON gfrv.ResourceVersionId = rv.Id
			INNER JOIN  resources.[File] f ON f.Id = gfrv.FileId
			LEFT OUTER JOIN resources.ResourceReference rr ON rr.ResourceId = rv.ResourceId AND rr.NodePathId = @NodePathId
			LEFT OUTER JOIN resources.ExternalReference er On er.ResourceReferenceId = rr.Id
			LEFT OUTER JOIN resources.UrlRewriting ur on ur.ExternalReferenceId = er.Id
			WHERE rv.id = @resourceVersionId
		END
		ELSE IF @ResourceTypeId = 12 -- Html resource
		BEGIN
			SELECT
					ISNULL(er.Id, 0) AS ExternalReferenceId,   
					hrv.EsrLinkTypeId EsrLinkType,
					CASE WHEN hrv.CreateUserId = @UserId
							THEN CAST(1 AS BIT) 
							ELSE ISNULL(@IsEditor, CAST(0 AS BIT))
					END  IsOwnerOrEditor,
					f.FilePath ContentFilePath,
					er.ExternalReference ExternalReference,
					f.[FileName] DefaultUrl,
					'' HostedContentUrl,
					ur.FullHistoricUrl
					--,srv.ClearSuspendData
			FROM resources.ResourceVersion rv
			INNER JOIN	resources.HtmlResourceVersion hrv ON hrv.ResourceVersionId = rv.Id
			INNER JOIN  resources.[File] f ON f.Id = hrv.FileId
			LEFT OUTER JOIN resources.ResourceReference rr ON rr.ResourceId = rv.ResourceId AND rr.NodePathId = @NodePathId
			LEFT OUTER JOIN resources.ExternalReference er On er.ResourceReferenceId = rr.Id
			LEFT OUTER JOIN resources.UrlRewriting ur on ur.ExternalReferenceId = er.Id
			WHERE rv.id = @resourceVersionId
		END
END