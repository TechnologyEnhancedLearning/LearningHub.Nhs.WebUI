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
CREATE PROCEDURE [resources].[GetContributionsDraft]
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
		0 AS ResourceReferenceId,
		rv.VersionStatusId,
		rv.ResourceAccessibilityId,
		rv.Title, 
		r.ResourceTypeId, 
		CAST(1 AS BIT) AS InEdit, 
		rv.Id AS DraftResourceVersionId, 
		rv.CreateDate AS CreatedDate, 
		NULL AS PublishedDate, 
		CAST(0 AS BIT) AS UnpublishedByAdmin,
		rv.AmendDate,
		CAST(0 AS BIT)  AS HasValidationErrors
	FROM	
		resources.ResourceVersion rv
		INNER JOIN	resources.Resource r ON rv.ResourceId = r.Id
		LEFT OUTER JOIN	hierarchy.NodeResource nr ON rv.ResourceId = nr.ResourceId AND nr.VersionStatusId = 1 AND nr.Deleted = 0
		LEFT OUTER JOIN  hierarchy.NodePath np ON np.NodeId = nr.NodeId AND np.Deleted = 0 AND np.isActive = 1
	WHERE	
		r.Deleted = 0 AND rv.Deleted = 0
		AND ISNULL(np.CatalogueNodeId, 1) = @CatalogueNodeId
		AND (rv.AmendUserId = @UserId OR @RestrictToCurrentUser = 0)
		AND rv.VersionStatusId = 1 /* Draft */
		AND r.CurrentResourceVersionId IS NULL
	ORDER BY rv.AmendDate DESC
	OFFSET @Offset ROWS
	FETCH NEXT @Take ROWS ONLY

END