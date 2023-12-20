-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      09-08-2021
-- Purpose      Discards a Hierarchy Edit.
--
-- Modification History
--
-- 25-08-2021  KD	Initial Revision.
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

END
GO