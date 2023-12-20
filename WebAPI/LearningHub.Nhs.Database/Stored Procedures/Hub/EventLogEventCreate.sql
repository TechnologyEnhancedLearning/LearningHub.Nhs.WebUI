-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Create an Event Log Event.
--
-- Modification History
--
-- 04-10-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [hub].[EventLogEventCreate]
(
	@EventLogId int,
	@EventTypeId int,
	@HierarchyEditId int,
	@NodeId int,
	@ResourceVersionId int,
	@Details nvarchar(1024),
	@UserId int
)

AS

BEGIN
		
	INSERT INTO [hub].[Event] ([EventLogId],[EventTypeId],[HierarchyEditId],[NodeId],[ResourceVersionId],[Details],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	SELECT @EventLogId, @EventTypeId, @HierarchyEditId, @NodeId, @ResourceVersionId, @Details, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET()

END

GO