-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Creates a Folder node version.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[FolderNodeVersionCreate]
(
	@FolderName NVARCHAR(128),
	@FolderDescription NVARCHAR(4000),
	@CreatedNodeVersionId INT output
)

AS

BEGIN

	-- Node
	DECLARE @CreatedNodeId INT

	INSERT INTO hierarchy.[Node] (NodeTypeId, CurrentNodeVersionId, Name, Description, Hidden, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (3, null, 'Folder', 'Folder', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

	SELECT @CreatedNodeId = SCOPE_IDENTITY()

	-- NodeVersion
	DECLARE @NodeVersionId int
	INSERT INTO hierarchy.NodeVersion (NodeId, VersionStatusId, PublicationId, MajorVersion, MinorVersion, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@CreatedNodeId, 1, NULL, NULL, NULL, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()) -- Draft VersionStatusId=1
	SELECT @CreatedNodeVersionId = SCOPE_IDENTITY()

	-- FolderNodeVersion
	INSERT INTO hierarchy.FolderNodeVersion (NodeVersionId, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@CreatedNodeVersionId, @FolderName, @FolderDescription, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

END
GO