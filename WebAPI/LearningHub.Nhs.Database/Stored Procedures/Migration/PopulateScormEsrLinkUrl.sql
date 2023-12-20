-------------------------------------------------------------------------------
-- Author       RobS
-- Created      06-04-2020
-- Purpose      Creates a record in the UrlRewriting table that stores the external ESR Link URL of a SCORM resource.
--				Does nothing if the resource version is not SCORM.
--
-- Modification History
--
-- 06-04-2020  RobS	- Initial Revision.
-- 27-04-2020  RobS - Added population of PackageRootUrl.
-------------------------------------------------------------------------------
CREATE PROCEDURE [migrations].[PopulateScormEsrLinkUrl]
(
	@ResourceVersionId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())
		
	-- Create a new row in the UrlRewriting table, using the Id of the ESR ResourceReference and not the LH catalogue ResourceReference.
	-- The ESR link URL comes from the MigrationInputRecord. It gets pulled from the input data and stored in its own column during the validation stage.
	-- If the resource is not SCORM, this insert doesn't do anything.
	INSERT INTO [resources].[UrlRewriting] (ExternalReferenceId, FullHistoricUrl, PackageRootUrl, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	SELECT
		er.Id, 
		mir.ScormEsrLinkUrl, 
		LEFT(mir.ScormEsrLinkUrl, LEN(mir.ScormEsrLinkUrl) - LEN(srvm.ManifestURL) - 1),
		0, 
		@UserId, 
		@AmendDate, 
		@UserId, 
		@AmendDate
	FROM [resources].[Resource] r
		LEFT JOIN [resources].[ResourceReference] rr ON r.Id = rr.ResourceId
		LEFT JOIN [resources].[ExternalReference] er ON rr.Id = er.ResourceReferenceId
		LEFT JOIN [hierarchy].[NodePath] np ON rr.NodePathId = np.Id
		LEFT JOIN [hierarchy].[Node] n ON np.NodeId = n.Id
		LEFT JOIN [migrations].[MigrationInputRecord] mir ON mir.ResourceVersionId = @ResourceVersionId
		LEFT JOIN [resources].[ScormResourceVersion] srv ON srv.ResourceVersionId = @ResourceVersionId
		LEFT JOIN [resources].[ScormResourceVersionManifest] srvm ON srvm.ScormResourceVersionId = srv.Id
	WHERE 
		r.ResourceTypeId = 6 AND -- Scorm
		n.NodeTypeId = 4 AND n.[Name] = 'ESR' AND 
		r.CurrentResourceVersionId = @ResourceVersionId AND
		mir.ScormEsrLinkUrl IS NOT NULL

END