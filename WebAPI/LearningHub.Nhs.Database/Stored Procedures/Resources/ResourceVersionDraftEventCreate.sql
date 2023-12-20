-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Create a "Create Draft" Resource Version Event.
--
-- Modification History
--
-- 01-01-2020  Killian Davies	Initial Revision
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionDraftEventCreate]
(
	@ResourceVersionId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
		
	DECLARE @ResourceVersionEventTypeId int=1
	DECLARE @Details nvarchar(1024)='Initial draft created'

	EXECUTE [resources].[ResourceVersionEventCreate]  @ResourceVersionId, @ResourceVersionEventTypeId, @Details, @UserId, @UserTimezoneOffset

END
GO

