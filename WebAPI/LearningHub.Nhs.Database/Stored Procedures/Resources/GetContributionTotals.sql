-------------------------------------------------------------------------------
-- Author       DJB
-- Created      20 Oct 2020
-- Purpose      Gets the contribution totals for the user or catalogue
--
-- Modification History
--
-- 23 Oct 2020	DJB	Initial Revision
-- 11 Dec 2020	DJB	UserAmmounts added
-- 30 Sep 2021  RS  Changes for content structure - look in subfolders too
-- 05 Oct 2021  RS  Restructured to use temp table for better performance.
-- 17 Dec 2021  RS  Fix for unpublished count - ignore old unpublished versions.
-- 20 Sep 2022  HV	Changes to show UnPublished by Admin.
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[GetContributionTotals]
	@CatalogueNodeId	int,
	@UserId				int,
	@ActionRequiredCount		int OUTPUT,
	@UserActionRequiredCount	int OUTPUT,
	@DraftCount					int OUTPUT,
	@UserDraftCount				int OUTPUT,
	@PublishedCount				int OUTPUT,
	@UserPublishedCount			int OUTPUT,
	@UnpublishedCount			int OUTPUT,
	@UserUnpublishedCount		int OUTPUT
AS
BEGIN
	
	SELECT	
			r.Id AS ResourceId,
			r.CurrentResourceVersionId,
			rv.Id AS ResourceVersionId, 
			rr.Id AS ResourceReferenceId,
			rv.VersionStatusId AS ResourceVersionStatusId, 
			rv.AmendUserId,
			rvp.Id AS ResourceVersionIdPublishing,
			nr.VersionStatusId AS NodeResourceVersionStatusId,
			nr.Deleted AS NodeResourceDeleted,
			np.Deleted AS NodePathDeleted,
			rve.Id AS UnpublishByAdminResourceVersionEventId
	INTO	#Resources
	FROM	resources.ResourceVersion rv
			INNER JOIN	resources.Resource r ON rv.ResourceId = r.Id
			LEFT OUTER JOIN	hierarchy.NodeResource nr ON rv.ResourceId = nr.ResourceId AND nr.Deleted = 0
			LEFT OUTER JOIN  hierarchy.NodePath np ON np.NodeId = nr.NodeId AND np.Deleted = 0 AND np.isActive = 1
			LEFT OUTER JOIN	resources.ResourceReference rr ON r.Id = rr.ResourceId AND np.Id = rr.NodePathId AND rr.Deleted = 0
			LEFT OUTER JOIN resources.ResourceVersion rvp ON r.Id = rvp.ResourceId AND (rvp.VersionStatusId = 4 /* Publishing */ OR rvp.VersionStatusId = 5 /* SubmittedToPublishingQueue */) AND rvp.Deleted = 0
			LEFT OUTER JOIN resources.ResourceVersionEvent rve ON rve.ResourceVersionId = rv.Id AND rve.ResourceVersionEventTypeId = 6
	WHERE	r.Deleted = 0 AND rv.Deleted = 0
			AND ISNULL(np.CatalogueNodeId, 1) = @CatalogueNodeId
			AND (@CatalogueNodeId > 1 OR rv.AmendUserId = @UserId)

	SELECT	@ActionRequiredCount = ISNULL(COUNT(DISTINCT ResourceId), 0)
	FROM	#Resources rv
	WHERE	NodeResourceDeleted = 0
			AND ResourceVersionStatusId = 6 /* FailedToPublish */
			OR (ResourceVersionStatusId in (2, 3) AND CurrentResourceVersionId = ResourceVersionId AND UnpublishByAdminResourceVersionEventId IS NOT NULL) /* Published or Unpublished and Flagged */ 
			AND (SELECT COUNT(*) FROM #Resources rv2 WHERE rv2.ResourceId = rv.ResourceId AND rv2.ResourceVersionStatusId IN (4,5)) < 1

	SELECT	@DraftCount = ISNULL(COUNT(ResourceId), 0)
	FROM	#Resources
	WHERE	ResourceVersionStatusId = 1 /* Draft */
			AND CurrentResourceVersionId IS NULL

	SELECT	@PublishedCount = ISNULL(COUNT(ResourceId), 0)
	FROM	#Resources
	WHERE	NodeResourceDeleted = 0 AND NodePathDeleted = 0
			AND (
				(ResourceVersionStatusId = 2 /* Publish */ AND CurrentResourceVersionId = ResourceVersionId AND ResourceReferenceId IS NOT NULL AND ResourceVersionIdPublishing IS NULL AND NodeResourceVersionStatusId = 2 /* Publish */)
				OR (ResourceVersionStatusId = 4 /* Publishing */ AND NodeResourceVersionStatusId != 3 /* Unpublished */)
				OR (ResourceVersionStatusId = 5 /* SubmittedToPublishingQueue */ AND NodeResourceVersionStatusId != 3 /* Unpublished */)
			)
			AND UnpublishByAdminResourceVersionEventId IS NULL -- not unpublished by admin

	SELECT	@UnpublishedCount = ISNULL(COUNT(ResourceVersionId), 0)
	FROM	#Resources rv
	WHERE	NodeResourceDeleted = 0
			AND ResourceVersionStatusId = 3 /* Unpublished */
			AND (SELECT COUNT(*) FROM #Resources rv2 WHERE rv2.ResourceId = rv.ResourceId AND rv2.ResourceVersionStatusId IN (4,5)) < 1			
			AND CurrentResourceVersionId = ResourceVersionId -- ignore old unpublished resource versions
			AND UnpublishByAdminResourceVersionEventId IS NULL -- not unpublished by admin

	IF (@CatalogueNodeId > 1)
	BEGIN
		SELECT	@UserActionRequiredCount = ISNULL(COUNT(DISTINCT ResourceVersionId), 0)
		FROM	#Resources rv
		WHERE	AmendUserId = @UserId
				AND NodeResourceDeleted = 0
				AND (ResourceVersionStatusId = 6 /* FailedToPublish */
				OR (ResourceVersionStatusId in (2, 3) AND CurrentResourceVersionId = ResourceVersionId AND UnpublishByAdminResourceVersionEventId IS NOT NULL)) /* Published or Unpublished and Flagged */ 
				AND (SELECT COUNT(*) FROM #Resources rv2 WHERE rv2.ResourceId = rv.ResourceId AND rv2.ResourceVersionStatusId IN (4,5)) < 1

		SELECT	@UserDraftCount = ISNULL(COUNT(ResourceId), 0)
		FROM	#Resources
		WHERE	AmendUserId = @UserId
				AND ResourceVersionStatusId = 1 /* Draft */
				AND CurrentResourceVersionId IS NULL

		SELECT	@UserPublishedCount = ISNULL(COUNT(ResourceId), 0)
		FROM	#Resources
		WHERE	AmendUserId = @UserId
				AND NodeResourceDeleted = 0 AND NodePathDeleted = 0
				AND (
					(ResourceVersionStatusId = 2 /* Publish */ AND CurrentResourceVersionId = ResourceVersionId AND ResourceReferenceId IS NOT NULL AND ResourceVersionIdPublishing IS NULL AND NodeResourceVersionStatusId = 2 /* Publish */)
					OR (ResourceVersionStatusId = 4 /* Publishing */ AND NodeResourceVersionStatusId != 3 /* Unpublished */)
					OR (ResourceVersionStatusId = 5 /* SubmittedToPublishingQueue */ AND NodeResourceVersionStatusId != 3 /* Unpublished */)
				)
				AND UnpublishByAdminResourceVersionEventId IS NULL -- not unpublished by admin

		SELECT	@UserUnpublishedCount = ISNULL(COUNT(ResourceVersionId), 0)
		FROM	#Resources rv
		WHERE	AmendUserId = @UserId
				AND NodeResourceDeleted = 0
				AND ResourceVersionStatusId = 3 /* Unpublished */
				AND (SELECT COUNT(*) FROM #Resources rv2 WHERE rv2.ResourceId = rv.ResourceId AND rv2.ResourceVersionStatusId IN (4,5)) < 1				
				AND UnpublishByAdminResourceVersionEventId IS NULL -- not unpublished by admin
				AND CurrentResourceVersionId = ResourceVersionId -- ignore old unpublished resource versions
	END
	ELSE
	BEGIN
		SET @UserActionRequiredCount = @ActionRequiredCount;
		SET @UserDraftCount = @DraftCount;
		SET @UserPublishedCount = @PublishedCount;
		SET @UserUnpublishedCount = @UnpublishedCount;
	END

	DROP TABLE #Resources

END