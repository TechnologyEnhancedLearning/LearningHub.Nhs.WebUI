-------------------------------------------------------------------------------
-- Author       DJB
-- Created      20 Oct 2020
-- Purpose      Gets the contributions for the user or catalogue
--
-- Modification History
--
-- 20 Oct 2020	DJB	Initial Revision
-- 11 Dec 2020	DJB	Fix for Action Required to ignor previous Resource Versions
-- 01 Oct 2021  RS  Changes for content structure - look in subfolders too
-- 13 Oct 2021  RS  Split out into separate proc for each status to improve SQL Server query plan caching. Removed temp table as not needed.
-- 20 Sep 2022  HV	Changes to show UnPublished by Admin.
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[GetContributionsPublished]
	@CatalogueNodeId		INT,
	@UserId					INT,
	@Offset					INT,
	@Take					INT,
	@RestrictToCurrentUser	BIT
AS
BEGIN

	DECLARE @EditorRoleId int = 1 -- Editor Role
	DECLARE @ScopeId int
	SELECT @ScopeId = Id FROM hub.Scope WHERE CatalogueNodeId = @CatalogueNodeId AND Deleted = 0

	-- Check the user is an editor of the Catalogue
	IF @CatalogueNodeId != 1 AND hub.UserIsInRole(@UserId, @EditorRoleId, @ScopeId) = 0
	BEGIN
		RAISERROR ('Access to catalogue denied',
				16,	-- Severity.  
				1	-- State.  
				); 
	END

	IF @CatalogueNodeId = 1
	BEGIN
		SET @RestrictToCurrentUser = 1;
	END

	SELECT	
		ROW_NUMBER() OVER(ORDER BY rv.AmendDate DESC) AS Id,
		rv.ResourceId, 
		rv.Id AS ResourceVersionId, 
		ISNULL(rr.OriginalResourceReferenceId, 0) AS ResourceReferenceId, 
		rv.VersionStatusId,
		rv.ResourceAccessibilityId,
		rv.Title,
		r.ResourceTypeId,
		CAST(case
			when rvd.id IS NULL then 0
			else 1
		end AS BIT) AS InEdit, 
		ISNULL(rvd.Id, rv.Id) AS DraftResourceVersionId, 
		rv.CreateDate AS CreatedDate, 
		p.CreateDate AS PublishedDate, 
		CAST(0 AS BIT) AS UnpublishedByAdmin,
		rv.AmendDate,
		CAST(0 AS BIT)  AS HasValidationErrors
	FROM	
		resources.ResourceVersion rv
		INNER JOIN	resources.Resource r ON rv.ResourceId = r.Id
		INNER JOIN	hierarchy.NodeResource nr ON rv.ResourceId = nr.ResourceId AND nr.Deleted = 0
		INNER JOIN  hierarchy.NodePath np ON np.NodeId = nr.NodeId AND np.Deleted = 0 AND np.isActive = 1 AND np.CatalogueNodeId = @CatalogueNodeId
		LEFT OUTER JOIN	resources.ResourceReference rr ON r.Id = rr.ResourceId AND np.Id = rr.NodePathId AND rr.Deleted = 0
		LEFT OUTER JOIN	hierarchy.Publication p ON rv.PublicationId = p.Id
		LEFT OUTER JOIN resources.ResourceVersion rvd ON r.Id = rvd.ResourceId AND rvd.VersionStatusId = 1 /* Draft */ AND rvd.Deleted = 0
		LEFT OUTER JOIN resources.ResourceVersion rvp ON r.Id = rvp.ResourceId AND (rvp.VersionStatusId = 4 /* Publishing */ OR rvp.VersionStatusId = 5 /* SubmittedToPublishingQueue */) AND rvp.Deleted = 0
		LEFT OUTER JOIN	resources.ResourceVersionFlag rvf ON rv.id = rvf.ResourceVersionId AND rvf.IsActive = 1
	WHERE	
		r.Deleted = 0 AND rv.Deleted = 0 AND nr.Deleted = 0 AND np.Deleted = 0
		AND (rv.AmendUserId = @UserId OR @RestrictToCurrentUser = 0)
		AND (  (rv.VersionStatusId = 2 /* Publish */ AND r.CurrentResourceVersionId = rv.id AND rr.id IS NOT NULL AND rvp.Id IS NULL AND nr.VersionStatusId = 2 /* Publish */)
			OR (rv.VersionStatusId = 4 /* Publishing */ AND nr.VersionStatusId != 3 /* Unpublished */)
			OR (rv.VersionStatusId = 5 /* SubmittedToPublishingQueue */ AND nr.VersionStatusId != 3 /* Unpublished */)
			)
		AND rvf.id IS NULL -- not flagged
	ORDER BY rv.AmendDate DESC
	OFFSET @Offset ROWS
	FETCH NEXT @Take ROWS ONLY

END