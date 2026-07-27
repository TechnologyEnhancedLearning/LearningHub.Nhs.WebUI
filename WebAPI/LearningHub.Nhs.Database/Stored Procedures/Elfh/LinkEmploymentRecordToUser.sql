-----------------------------------------------------------------------------------------------
-- Mark Avey 11/06/2020 - Initial Version.
-- Update the UserTBL record with the primaryUserEmploymentId from userEmploymentTBL
------------------------------------------------------------------------------------------------
CREATE PROCEDURE [elfh].[proc_LinkEmploymentRecordToUser]
(
    @userId int
)
AS
BEGIN
    BEGIN TRANSACTION
        BEGIN TRY
            SET NOCOUNT OFF
            
            UPDATE [hub].[User]
            SET primaryUserEmploymentId = (SELECT userEmploymentId FROM [elfh].[userEmploymentTBL] WHERE userId = @userId)
            WHERE Id = @userId

        END TRY

        BEGIN CATCH
            SELECT 
                ERROR_NUMBER() AS ErrorNumber
                ,ERROR_SEVERITY() AS ErrorSeverity
                ,ERROR_STATE() AS ErrorState
                ,ERROR_PROCEDURE() AS ErrorProcedure
                ,ERROR_LINE() AS ErrorLine
                ,ERROR_MESSAGE() AS ErrorMessage;
            
            IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION
        END CATCH

    IF @@TRANCOUNT > 0 COMMIT TRANSACTION

    RETURN @@ERROR
    END