WITH CTE AS (
    SELECT
        id,userid, usergroupid,
        ROW_NUMBER() OVER (PARTITION BY userid, usergroupid ORDER BY id) AS rn
    FROM
        [hub].[UserUserGroup] where Deleted = 0
)
DELETE FROM CTE WHERE rn > 1;
GO