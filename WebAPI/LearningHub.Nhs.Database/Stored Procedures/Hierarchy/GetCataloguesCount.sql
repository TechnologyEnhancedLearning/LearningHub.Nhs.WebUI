CREATE PROCEDURE [hierarchy].[GetCataloguesCount] (
	 @userId INT
	)
AS
BEGIN

	WITH  Catalogues AS(
			SELECT cnv.Name as Name
			FROM [hierarchy].[Node] n
			JOIN [hierarchy].[NodeVersion] nv ON nv.NodeId = n.Id
			JOIN [hierarchy].[CatalogueNodeVersion] cnv ON cnv.NodeVersionId = nv.Id
			LEFT JOIN hub.UserBookmark ub ON ub.UserId = @userId AND ub.NodeId = nv.NodeId
			LEFT JOIN (  SELECT DISTINCT CatalogueNodeId 
						  FROM [hub].[RoleUserGroupView] rug JOIN hub.UserUserGroup uug ON rug.UserGroupId = uug.UserGroupId
						  WHERE rug.ScopeTypeId = 1 and rug.RoleId in (1,2,3) and uug.Deleted = 0 and uug.UserId = @userId) auth ON n.Id = auth.CatalogueNodeId
			WHERE n.Id <> 1	AND n.Hidden = 0	AND n.Deleted = 0	AND cnv.Deleted = 0 AND nv.VersionStatusId = 2 ),


Alphabet AS (
    SELECT 'A' AS Letter UNION ALL
    SELECT 'B' UNION ALL
    SELECT 'C' UNION ALL
    SELECT 'D' UNION ALL
    SELECT 'E' UNION ALL
    SELECT 'F' UNION ALL
    SELECT 'G' UNION ALL
    SELECT 'H' UNION ALL
    SELECT 'I' UNION ALL
    SELECT 'J' UNION ALL
    SELECT 'K' UNION ALL
    SELECT 'L' UNION ALL
    SELECT 'M' UNION ALL
    SELECT 'N' UNION ALL
    SELECT 'O' UNION ALL
    SELECT 'P' UNION ALL
    SELECT 'Q' UNION ALL
    SELECT 'R' UNION ALL
    SELECT 'S' UNION ALL
    SELECT 'T' UNION ALL
    SELECT 'U' UNION ALL
    SELECT 'V' UNION ALL
    SELECT 'W' UNION ALL
    SELECT 'X' UNION ALL
    SELECT 'Y' UNION ALL
    SELECT 'Z' UNION ALL
    SELECT '0-9'
)
SELECT
				Alphabet.Letter AS Alphabet,
				COALESCE(COUNT(cnv.Name), 0) AS Count
				FROM Alphabet
			LEFT JOIN Catalogues cnv 
			ON
			((LEFT(cnv.Name, 1) = Alphabet.Letter AND LEFT(cnv.Name, 1) BETWEEN 'A' AND 'Z')
			OR (Alphabet.Letter = '0-9' AND LEFT(cnv.Name, 1) BETWEEN '0' AND '9')
			)
			
			GROUP BY Alphabet.Letter
			ORDER BY (CASE WHEN Alphabet.Letter like '[a-z]%' THEN 0 ELSE 1 END), Alphabet.Letter;
		
END
