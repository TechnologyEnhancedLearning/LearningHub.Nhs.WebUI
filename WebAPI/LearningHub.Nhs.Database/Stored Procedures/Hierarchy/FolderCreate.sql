-------------------------------------------------------------------------------
-- Author       RS
-- Created      09-08-2021
-- Purpose      Creates a Folder node.
--
-- Modification History
--
-- 09-08-2021  RS	Initial Revision for creating some dummy data. WIP!
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[FolderCreate]
(
	@ParentNodeId INT, -- NodeId of parent node - the catalogue or folder.
	@FolderName NVARCHAR(128),
	@FolderDescription NVARCHAR(4000),
	@CreatedNodeId INT output
)

AS

BEGIN
	-----------------------------------------------------------------------------------------------------------------------
	-- This is just a stored proc I created to help create some dummy folder structure data. It could be used as a starting 
	-- point when the work on creating/moving folders starts though.
	-----------------------------------------------------------------------------------------------------------------------

	-- Node
	INSERT INTO hierarchy.[Node] (NodeTypeId, CurrentNodeVersionId, Name, Description, Hidden, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (3, null, 'Folder', 'Folder', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
	SELECT @CreatedNodeId = SCOPE_IDENTITY()

	-- NodeVersion
	DECLARE @NodeVersionId int
	INSERT INTO hierarchy.NodeVersion (NodeId, VersionStatusId, PublicationId, MajorVersion, MinorVersion, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@CreatedNodeId, 2, NULL, 1, 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())
	SELECT @NodeVersionId = SCOPE_IDENTITY()
	UPDATE hierarchy.[Node] SET CurrentNodeVersionId = @NodeVersionId where Id = @CreatedNodeId

	-- FolderNodeVersion
	INSERT INTO hierarchy.FolderNodeVersion (NodeVersionId, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@NodeVersionId, @FolderName, @FolderDescription, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

	-- NodeLink
	DECLARE @DisplayOrder INT 
	SELECT @DisplayOrder = MAX(DisplayOrder) FROM 
		(SELECT MAX(DisplayOrder) AS DisplayOrder FROM hierarchy.NodeLink WHERE ParentNodeId = @ParentNodeId and Deleted = 0
			UNION
		SELECT MAX(DisplayOrder) AS DisplayOrder FROM hierarchy.NodeResource WHERE NodeId = @ParentNodeId and Deleted = 0) as t1

	INSERT INTO hierarchy.NodeLink (ParentNodeId, ChildNodeId, DisplayOrder, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@ParentNodeId, @CreatedNodeId, COALESCE(@DisplayOrder, 0) + 1, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

	-- NodePath
	-- Todo!

	-- NodePathNode
	-- Todo!

END