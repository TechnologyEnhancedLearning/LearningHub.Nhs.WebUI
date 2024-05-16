-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Discards a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 24-04-2024  DB	Delete draft NodeVersion and Node records that were created by this edit.
-- 15-05-2024  DB	Delete NodePath and ResourceReference records that were created by this edit.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditDiscard]
(
	@HierarchyEditId int,
	@AmendUserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN

	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

	UPDATE 
		hierarchy.HierarchyEdit
	SET
		[HierarchyEditStatusId] = 3,
		AmendUserId = @AmendUserId,
		AmendDate = @AmendDate
	WHERE
		[Id] = @HierarchyEditId

	-- Delete and draft FolderNodeVersion records that were created by this edit
	UPDATE fnv
	Set 
		Deleted = 1,
		AmendUserId = @AmendUserId,
		AmendDate = @AmendDate
	FROM hierarchy.FolderNodeVersion fnv
	INNER JOIN hierarchy.NodeVersion nv ON fnv.NodeVersionId = nv.Id
	INNER JOIN hierarchy.HierarchyEditDetail hed ON nv.NodeId = hed.NodeId
	WHERE hed.HierarchyEditId = @HierarchyEditId
		AND nv.VersionStatusId = 1 -- Draft
		AND fnv.Deleted = 0

	-- Delete and draft NodeVersion records that were created by this edit
	UPDATE nv
	Set 
		Deleted = 1,
		AmendUserId = @AmendUserId,
		AmendDate = @AmendDate
	FROM hierarchy.NodeVersion nv
	INNER JOIN hierarchy.HierarchyEditDetail hed ON nv.NodeId = hed.NodeId
	WHERE hed.HierarchyEditId = @HierarchyEditId
		AND nv.VersionStatusId = 1 -- Draft
		AND nv.Deleted = 0

	-- Delete any Node records that were created by this edit
	UPDATE n
	Set 
		Deleted = 1,
		AmendUserId = @AmendUserId,
		AmendDate = @AmendDate
	FROM hierarchy.Node n
	INNER JOIN hierarchy.HierarchyEditDetail hed ON n.Id = hed.NodeId
	WHERE hed.HierarchyEditId = @HierarchyEditId
		  AND hed.HierarchyEditDetailTypeId = 3 -- Folder Node
		  AND hed.HierarchyEditDetailOperationId = 1 -- Add

	-- Delete NodePath records that were created by this edit for New References
	UPDATE np
	Set 
		Deleted = 1,
		AmendUserId = @AmendUserId,
		AmendDate = @AmendDate
	FROM hierarchy.NodePath np
	INNER JOIN hierarchy.HierarchyEditDetail hed ON np.Id = hed.NodePathId
	WHERE hed.HierarchyEditId = @HierarchyEditId
		  AND hed.HierarchyEditDetailTypeId = 4 -- Node Link
		  AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference
		  AND np.IsActive = 0

	-- Delete ResourceReference records that were created by this edit for New References
	UPDATE rr
	Set 
		Deleted = 1,
		AmendUserId = @AmendUserId,
		AmendDate = @AmendDate
	FROM resources.ResourceReference rr
	INNER JOIN hierarchy.HierarchyEditDetail hed ON rr.Id = hed.ResourceReferenceId
	WHERE hed.HierarchyEditId = @HierarchyEditId
		  AND hed.HierarchyEditDetailTypeId = 5 -- Node Resource
		  AND hed.HierarchyEditDetailOperationId = 4 -- Add Reference

END
GO