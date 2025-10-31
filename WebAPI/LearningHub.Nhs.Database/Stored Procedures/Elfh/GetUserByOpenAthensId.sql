-----------------------------------------------------------------------------------------------
-- Tobi Awe 31/10/2025 - Initial Version.
-- converted elfh efcore query to sp in LH database
------------------------------------------------------------------------------------------------

CREATE PROCEDURE elfh.proc_GetUserByOpenAthensId
    @OpenAthensId varchar(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT 
        u.Id,
        u.UserName,
        up.FirstName,
        up.LastName,
        up.EmailAddress,
        u.AmendDate AS LastUpdated
    FROM hub.[user] u
    INNER JOIN hub.userprofile up ON up.Id = u.Id
    INNER JOIN elfh.userAttributeTBL ua ON ua.UserId = u.Id
    WHERE ua.TextValue = @OpenAthensId AND u.deleted = 0 AND up.deleted = 0 AND ua.deleted = 0;
END
