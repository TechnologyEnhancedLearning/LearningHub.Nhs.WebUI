-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      23-02-2021
-- Purpose      Loads e-LfH "user - user groups" from staging
--
-- Modification History
--
-- 23-02-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [adf].[LoadElfhUserUserGroups]
(
	@JobRunId int
)

AS
BEGIN

	DECLARE @createdCount int = 0
	DECLARE @updatedCount int = 0
	DECLARE @unmappedCount int = 0

	BEGIN TRY

		-- Update existing User - User Group/s ("soft delete")
		UPDATE
			uug
		SET
			Deleted = elfh_userUserGroup.deleted,
			AmendUserId = 4, -- System User Id
			AmendDate = SYSDATETIMEOFFSET()
		FROM
			hub.[UserUserGroup] uug
		INNER JOIN 
			adf.Staging_Elfh_UserUserGroup elfh_userUserGroup 
				ON uug.UserGroupId = elfh_userUserGroup.userGroupId 
				AND  uug.UserId = elfh_userUserGroup.userId 
				AND (uug.Deleted != elfh_userUserGroup.deleted)
		WHERE
			elfh_userUserGroup.JobRunId = @JobRunId

		SELECT @updatedCount = @@ROWCOUNT

		-- Identify records that cannot be mapped due to missing FK references
		INSERT INTO adf.Unmapped_UserUserGroup (JobRunId, userUserGroupId, userId, userGroupId, deleted, amendDate, UserMissingInd, UserGroupMissingInd)
		SELECT 
			@JobRunId AS JobRunId,
			elfh_userUserGroup.userUserGroupId,
			elfh_userUserGroup.userId, 
			elfh_userUserGroup.userGroupId, 
			elfh_userUserGroup.deleted,
			elfh_userUserGroup.amendDate,
			CASE WHEN u.Id IS NULL THEN 1 ELSE 0 END AS UserMissingInd,
			CASE WHEN ug.Id IS NULL THEN 1 ELSE 0 END AS UserGroupMissingInd
		FROM
			adf.Staging_Elfh_UserUserGroup elfh_userUserGroup
		LEFT JOIN
			hub.[User] u ON elfh_userUserGroup.userId = u.Id
		LEFT JOIN
			hub.[UserGroup] ug ON elfh_userUserGroup.userGroupId = ug.Id
		WHERE
			JobRunId = @JobRunId
			AND (u.Id IS NULL OR ug.Id IS NULL)

		SELECT @unmappedCount = @@ROWCOUNT

		-- Create new User - User Group/s, where FK mapping is possible
		INSERT INTO [hub].[UserUserGroup](UserId, UserGroupId, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
		SELECT 
			elfh_userUserGroup.userId AS UserId, 
			elfh_userUserGroup.userGroupId AS UserGroupId, 
			0 AS Deleted,
			4 AS CreateUserId,
			SYSDATETIMEOFFSET() AS CreateDate,
			4 AS AmendUserId,
			SYSDATETIMEOFFSET() AS AmendDate
		FROM
			adf.Staging_Elfh_UserUserGroup elfh_userUserGroup
		LEFT JOIN
			hub.[UserUserGroup] uug ON elfh_userUserGroup.userGroupId = uug.UserGroupId AND elfh_userUserGroup.userId = uug.UserId
		LEFT JOIN
			adf.Unmapped_UserUserGroup unmapped 
				ON elfh_userUserGroup.JobRunId = unmapped.JobRunId
				AND elfh_userUserGroup.userId = unmapped.userId
				AND elfh_userUserGroup.userGroupId = unmapped.userGroupId
		WHERE
			elfh_userUserGroup.JobRunId = @JobRunId
			AND elfh_userUserGroup.deleted = 0
			AND uug.Id IS NULL
			ANd unmapped.userId IS NULL
			
		SELECT @createdCount = @@ROWCOUNT

		DECLARE @noActionCount int
		SELECT 
			@noActionCount = COUNT(elfh_userUserGroup.userId)
		FROM
			adf.Staging_Elfh_UserUserGroup elfh_userUserGroup
		LEFT JOIN
			hub.[UserUserGroup] uug ON elfh_userUserGroup.userGroupId = uug.UserGroupId AND elfh_userUserGroup.userId = uug.UserId
		LEFT JOIN
			adf.Unmapped_UserUserGroup unmapped 
				ON elfh_userUserGroup.JobRunId = unmapped.JobRunId
				AND elfh_userUserGroup.userId = unmapped.userId
				AND elfh_userUserGroup.userGroupId = unmapped.userGroupId
		WHERE
			elfh_userUserGroup.JobRunId = @JobRunId
			AND elfh_userUserGroup.deleted = 1
			AND uug.Id IS NULL
			ANd unmapped.userId IS NULL

		SELECT 
			CreatedCount = @createdCount, 
			UpdatedCount = @updatedCount,
			UnmappedCount = @unmappedCount

	END TRY
	BEGIN CATCH
	    DECLARE @ErrorMessage NVARCHAR(4000);  
		DECLARE @ErrorSeverity INT;  
		DECLARE @ErrorState INT;  
  
		SELECT   
			@ErrorMessage = ERROR_MESSAGE(),  
			@ErrorSeverity = ERROR_SEVERITY(),  
			@ErrorState = ERROR_STATE();  

		RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);  

	END CATCH

END
GO