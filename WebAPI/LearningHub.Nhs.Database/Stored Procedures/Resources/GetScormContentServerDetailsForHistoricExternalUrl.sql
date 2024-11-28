-------------------------------------------------------------------------------
-- Author       RobS
-- Created      19-04-2021
-- Purpose      Gets the SCORM content details required by the content server, to map historic external URLs to internal content folders.
--
-- Modification History
-- 
-- 19-04-2021	RobS	Initial Revision
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetScormContentServerDetailsForHistoricExternalUrl]
	@externalUrl		NVARCHAR(4000)
AS
BEGIN
		SELECT
			rv.Title,
			er.ResourceReferenceId,
			rv.VersionStatusId VersionStatus,
			srv.EsrLinkTypeId EsrLinkType,
			er.Active IsActive,
			srv.ContentFilePath AS InternalResourceIdentifier, 
			svm.ManifestURL AS DefaultUrl				
		FROM 
			[resources].[UrlRewriting] u  
			join resources.ExternalReference er on u.ExternalReferenceId = er.Id 
			join resources.ResourceReference rr on er.ResourceReferenceId = rr.Id
			join resources.Resource r on r.Id = rr.ResourceId
			join resources.ResourceVersion rv on rv.Id = r.CurrentResourceVersionId
			join resources.ScormResourceVersion srv on srv.ResourceVersionId = r.CurrentResourceVersionId
			join resources.ScormResourceVersionManifest svm on svm.ScormResourceVersionId = srv.Id
		WHERE 
			u.FullHistoricUrl LIKE '%'+ @externalUrl +'%'
			AND er.Deleted = 0
END