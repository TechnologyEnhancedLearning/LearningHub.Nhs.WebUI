-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Creates a Folder node version.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
-- 22-04-2024  DB	Included NodeId as an input so that editiing of a folder creates a new node version.
-- 10-07-2024  DB	Added PrimaryCatalogueNodeId to the NodeVersion table.
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[FolderNodeVersionCreate]
(
	@NodeId INT NULL,
	@FolderName NVARCHAR(128),
	@FolderDescription NVARCHAR(4000),
	@PrimaryCatalogueNodeId INT,
	@UserId INT,
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

	-- Get the current maximum MajorVersion and MinorVersion numbers for a node version for the current nodeid
	DECLARE @MajorVersion INT
	DECLARE @MinorVersion INT
	SELECT	@MajorVersion = ISNULL(MAX(MajorVersion), 1),
			@MinorVersion = ISNULL(MAX(MinorVersion), 0)
	FROM	hierarchy.NodeVersion
	WHERE	NodeId = @NodeId
		AND Deleted = 0

	--NodeVersion
	INSERT INTO hierarchy.NodeVersion (NodeId, VersionStatusId, PublicationId, MajorVersion, MinorVersion, PrimaryCatalogueNodeId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@NodeId, 1, NULL, @MajorVersion, @MinorVersion + 1, @PrimaryCatalogueNodeId, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET()) -- Draft VersionStatusId=1
	SELECT @CreatedNodeVersionId = SCOPE_IDENTITY()

	-- FolderNodeVersion
	INSERT INTO hierarchy.FolderNodeVersion (NodeVersionId, Name, Description, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
	VALUES (@CreatedNodeVersionId, @FolderName, @FolderDescription, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

END
GO