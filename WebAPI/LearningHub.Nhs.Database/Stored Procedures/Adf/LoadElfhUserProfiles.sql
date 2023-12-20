-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      23-02-2021
-- Purpose      Loads e-LfH users from staging
--
-- Modification History
--
-- 23-02-2021  Killian Davies	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [adf].[LoadElfhUserProfiles]
(
	@JobRunId int
)

AS
BEGIN


	BEGIN TRY

		DECLARE @usersCreatedCount int
		DECLARE @usersUpdatedCount int

		-- Update existing User/s
		UPDATE
			u
		SET
			UserName = elfh_user.userName,
			EmailAddress = elfh_user.emailAddress,
			FirstName = elfh_user.firstName,
			LastName = elfh_user.lastName,
			[Active] = elfh_user.active,
			Deleted = elfh_user.deleted,
			AmendUserId = 4,
			AmendDate = SYSDATETIMEOFFSET()
		FROM
			hub.[UserProfile] u
		INNER JOIN 
			adf.Staging_Elfh_UserProfile elfh_user 
				ON u.Id = elfh_user.userId 
				AND ((u.UserName != elfh_user.userName)
					OR (u.EmailAddress != elfh_user.emailAddress)
					OR (u.FirstName != elfh_user.firstName)
					OR (u.LastName != elfh_user.lastName)
					OR (u.Active != elfh_user.active)
					OR (u.Deleted != elfh_user.deleted))
		WHERE
			elfh_user.JobRunId = @JobRunId

		SELECT @usersUpdatedCount = @@ROWCOUNT

		-- Create new User Profile/s
		INSERT INTO hub.[UserProfile]([Id],[UserName],[EmailAddress],[FirstName],[LastName],[Active],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		SELECT 
			elfh_user.userId AS Id, 
			elfh_user.userName AS UserName, 
			elfh_user.emailAddress AS EmailAddress,
			elfh_user.firstName AS FirstName,
			elfh_user.lastName AS LastName,
			elfh_user.active AS [Active],
			0 AS Deleted,
			4 AS CreateUserId,
			SYSDATETIMEOFFSET() AS CreateDate,
			4 AS AmendUserId,
			SYSDATETIMEOFFSET() AS AmendDate
		FROM
			adf.Staging_Elfh_UserProfile elfh_user
		LEFT JOIN
			hub.[UserProfile] u ON elfh_user.userId = u.Id
		WHERE
			JobRunId = @JobRunId
			AND u.Id IS NULL
			
		SELECT @usersCreatedCount = @@ROWCOUNT

		SELECT 
			CreatedCount = @usersCreatedCount, 
			UpdatedCount = @usersUpdatedCount

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
