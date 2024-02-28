-------------------------------------------------------------------------------
-- Author       RobS
-- Created      19-04-2021
-- Purpose      Gets the SCORM content details required by the content server, to map LH external references to internal content folders.
--
-- Modification History
-- 
-- 19-04-2021	RobS	Initial Revision
-- 01-06-2021	Dave	ExternalReference Active flag required
-- 23 Oct 2023	Dave	Name changes from GetScormContentServerDetailsForLHExternalReference to GetContentServerDetailsForLHExternalReference
--						Updated to allow generic file resources
-------------------------------------------------------------------------------

CREATE PROCEDURE [resources].[GetContentServerDetailsForLHExternalReference]
	@externalReference		NVARCHAR(4000)
AS
BEGIN
	DECLARE @ResourceTypeId			INT
	DECLARE @ResourceVersionId		INT
	DECLARE @IsActive				BIT
	DECLARE @ResourceReferenceId	INT

	SELECT
		@ResourceReferenceId = er.ResourceReferenceId,
		@ResourceTypeId = r.ResourceTypeId,
		@ResourceVersionId = rv.Id,
		@IsActive = er.Active
	FROM 
		resources.ExternalReference er
		INNER JOIN resources.ResourceReference rr on er.ResourceReferenceId = rr.Id
		INNER JOIN resources.Resource r on r.Id = rr.ResourceId
		INNER JOIN resources.ResourceVersion rv on rv.Id = r.CurrentResourceVersionId
	WHERE 
		er.ExternalReference = @externalReference
		AND er.Deleted = 0


	IF @ResourceTypeId = 6 -- SCORM e-learning resource
	BEGIN
		SELECT
			rv.Title,
			@ResourceReferenceId ResourceReferenceId,
			rv.VersionStatusId VersionStatus,
			srv.EsrLinkTypeId EsrLinkType,
			@IsActive IsActive,
			srv.ContentFilePath AS InternalResourceIdentifier, 
			svm.ManifestURL AS DefaultUrl			
		FROM resources.ResourceVersion rv
			INNER JOIN resources.ScormResourceVersion srv on srv.ResourceVersionId = rv.Id
			INNER JOIN resources.ScormResourceVersionManifest svm on svm.ScormResourceVersionId = srv.Id
		WHERE
			rv.Id = @ResourceVersionId
	END
	ELSE IF @ResourceTypeId = 9 -- Generic File resource
	BEGIN
		SELECT
			rv.Title,
			@ResourceReferenceId ResourceReferenceId,
			rv.VersionStatusId VersionStatus,
			gfrv.EsrLinkTypeId EsrLinkType,
			@IsActive IsActive,
			f.FilePath AS InternalResourceIdentifier, 
			f.[FileName] AS DefaultUrl			
		FROM resources.ResourceVersion rv
			INNER JOIN resources.GenericFileResourceVersion gfrv on rv.Id = gfrv.ResourceVersionId
			INNER JOIN resources.[File] f on gfrv.FileId = f.Id
		WHERE
			rv.Id = @ResourceVersionId
	END
	ELSE IF @ResourceTypeId = 12 -- Html resource
	BEGIN
		SELECT
			rv.Title,
			@ResourceReferenceId ResourceReferenceId,
			rv.VersionStatusId VersionStatus,
			hrv.EsrLinkTypeId EsrLinkType,
			@IsActive IsActive,
			hrv.ContentFilePath AS InternalResourceIdentifier, 
			'index.html' AS DefaultUrl			
		FROM resources.ResourceVersion rv
			INNER JOIN resources.HtmlResourceVersion hrv on rv.Id = hrv.ResourceVersionId
			INNER JOIN resources.[File] f on hrv.FileId = f.Id
		WHERE
			rv.Id = @ResourceVersionId
	END
END
GO