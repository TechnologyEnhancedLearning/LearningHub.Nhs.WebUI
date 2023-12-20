-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      23-02-2021
-- Purpose      Loads e-LfH user groups from staging
--
-- Modification History
--
-- 23-02-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [adf].[LoadElfhUserGroups] 
(
	@JobRunId int
)

AS
BEGIN

	DECLARE @createdCount int = 0
	DECLARE @updatedCount int = 0

	BEGIN TRY

		-- Update existing User Group/s (including "soft delete") where there is a change to a mapped property
		UPDATE
			ug
		SET
			[Name] = elfh_userGroup.userGroupName,
			[Description] = elfh_userGroup.userGroupDescription,
			AmendUserId = 4,
			AmendDate = SYSDATETIMEOFFSET()
		FROM
			hub.[UserGroup] ug
		INNER JOIN 
			adf.Staging_Elfh_UserGroup elfh_userGroup 
				ON ug.Id = elfh_userGroup.userGroupId 
				AND (ug.[Name] != elfh_userGroup.userGroupName
					OR ug.[Description] != elfh_userGroup.userGroupDescription
					OR ug.Deleted != elfh_userGroup.deleted)
		WHERE
			elfh_userGroup.JobRunId = @JobRunId

		SELECT @updatedCount = @@ROWCOUNT

		-- Create new User Group/s
		-- note: lock table exclusively
		SET IDENTITY_INSERT [hub].[UserGroup] ON

		INSERT INTO [hub].[UserGroup]  WITH (TABLOCKX) ([Id],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT 
			elfh_userGroup.userGroupId AS Id, 
			elfh_userGroup.userGroupName AS [Name],
			elfh_userGroup.userGroupDescription AS [Description],  
			0 AS Deleted,
			4 AS CreateUserId,
			SYSDATETIMEOFFSET() AS CreateDate,
			4 AS AmendUserId,
			SYSDATETIMEOFFSET() AS AmendDate
		FROM
			adf.Staging_Elfh_UserGroup elfh_userGroup
		LEFT JOIN
			hub.[UserGroup] ug ON elfh_userGroup.userGroupId = ug.Id
		WHERE
			JobRunId = @JobRunId
			AND elfh_userGroup.deleted = 0
			AND ug.Id IS NULL

		SELECT @createdCount = @@ROWCOUNT

		SET IDENTITY_INSERT [hub].[UserGroup] OFF

		SELECT 
			CreatedCount = @createdCount, 
			UpdatedCount = @updatedCount

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