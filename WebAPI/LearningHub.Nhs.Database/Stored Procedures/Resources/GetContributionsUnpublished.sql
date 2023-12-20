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
CREATE PROCEDURE [resources].[GetContributionsUnpublished]
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
		rr.OriginalResourceReferenceId AS ResourceReferenceId, 
		rv.VersionStatusId,
		rv.ResourceAccessibilityId,
		rv.Title, 
		r.ResourceTypeId,
		CAST(case
			when rvd.id IS NULL then 0
			else 1
		end AS BIT) AS InEdit, 
		rvd.Id AS DraftResourceVersionId, 
		rv.CreateDate AS CreatedDate, 
		p.CreateDate AS PublishedDate, 
		CASE WHEN rv.VersionStatusId = 3 AND rve.Id IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END AS UnpublishedByAdmin,
		rv.AmendDate,
		CAST(0 AS BIT)  AS HasValidationErrors
	FROM	
		resources.ResourceVersion rv
		INNER JOIN	resources.Resource r ON rv.Id = r.CurrentResourceVersionId
		INNER JOIN	hierarchy.NodeResource nr ON rv.ResourceId = nr.ResourceId AND nr.Deleted = 0
		INNER JOIN  hierarchy.NodePath np ON np.NodeId = nr.NodeId AND np.Deleted = 0 AND np.isActive = 1
		INNER JOIN	resources.ResourceReference rr ON r.Id = rr.ResourceId AND rr.NodePathId = np.Id AND rr.Deleted = 0
		INNER JOIN	hierarchy.Publication p ON rv.PublicationId = p.Id
		LEFT JOIN   resources.ResourceVersionEvent rve ON rve.ResourceVersionId = rv.Id AND rve.ResourceVersionEventTypeId = 6 /* Unpublished by admin */
		LEFT OUTER JOIN resources.ResourceVersion rvd ON r.Id = rvd.ResourceId AND rvd.VersionStatusId = 1 /* Draft */ AND rvd.Deleted = 0
		LEFT OUTER JOIN	resources.ResourceVersionFlag rvf ON rv.id = rvf.ResourceVersionId AND rvf.IsActive = 1
	WHERE	
		r.Deleted = 0 AND rv.Deleted = 0
		AND np.CatalogueNodeId = @CatalogueNodeId
		AND (rv.AmendUserId = @UserId OR @RestrictToCurrentUser = 0)
		AND rv.VersionStatusId = 3 /* Unpublished */
		AND (SELECT COUNT(*) FROM resources.ResourceVersion rv2 WHERE rv2.ResourceId = rv.ResourceId AND rv2.VersionStatusId IN (4,5)) < 1		
		AND rvf.id IS NULL -- not flagged
	ORDER BY rv.AmendDate DESC
	OFFSET @Offset ROWS
	FETCH NEXT @Take ROWS ONLY

END

