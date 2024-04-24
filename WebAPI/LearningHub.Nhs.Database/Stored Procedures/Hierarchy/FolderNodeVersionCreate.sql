-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Creates a Folder node version.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 22-04-2024  DB	Included NodeId as an input so that editiing of a folder creates a new node version.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[FolderNodeVersionCreate]
(
	@NodeId INT NULL,
	@FolderName NVARCHAR(128),
	@FolderDescription NVARCHAR(4000),
	@CreatedNodeVersionId INT output
)

AS

BEGIN

	IF (@NodeId IS NULL)
	BEGIN
		INSERT INTO hierarchy.[Node] (NodeTypeId, CurrentNodeVersionId, Name, Description, Hidden, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		VALUES (3, null, 'Folder', 'Folder', 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

		SELECT @NodeId = SCOPE_IDENTITY()
	END

	INSERT INTO hierarchy.NodeVersion (NodeId, VersionStatusId, PublicationId, MajorVersion, MinorVersion, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@NodeId, 1, NULL, NULL, NULL, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET()) -- Draft VersionStatusId=1
	SELECT @CreatedNodeVersionId = SCOPE_IDENTITY()

	-- FolderNodeVersion
	INSERT INTO hierarchy.FolderNodeVersion (NodeVersionId, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@CreatedNodeVersionId, @FolderName, @FolderDescription, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET())

END
GO