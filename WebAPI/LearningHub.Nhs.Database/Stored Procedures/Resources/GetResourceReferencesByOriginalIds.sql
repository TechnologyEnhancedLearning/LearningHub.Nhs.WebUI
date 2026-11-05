-------------------------------------------------------------------------------
-- Author       Swapnamol 28-09-202601-01-2020
-- Purpose      GetResourceRefrences by ID
--
-- Modification History
--
-- 28-09-2026  SA	Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[GetResourceReferencesByOriginalIds]
(
    @OriginalResourceReferenceIds dbo.IntIdsTableList READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

SELECT  rr.Id,
        rr.ResourceId AS ResourceId,
		rr.OriginalResourceReferenceId AS OriginalResourceReferenceId,
		r.CurrentResourceVersionId AS  CurrentResourceVersionId,
		r.ResourceTypeId AS ResourceTypeId,
		rv.Title AS Title,
		rv.[Description] AS Description,
		rv.MajorVersion AS MajorVersion,
		rv.MinorVersion AS MinorVersion,
		nv.NodeId AS CatalogueId,
		cnv.Name AS CatalogueName,
		cnv.RestrictedAccess AS RestrictedAccess,
		rvs.AverageRating AS AverageRating,
		rr.CreateDate,
		rr.CreateUserId,
		rr.AmendUserId,
		rr.AmendDate,
		rr.Deleted
		FROM resources.ResourceReference AS RR
		LEFT JOIN [hierarchy].[NodePath] AS NP
		   ON RR.[NodePathId] = [np].[Id] AND [np].[Deleted] = 0
		LEFT JOIN [hierarchy].[Node] AS N
		ON NP.[NodeId] = N.[Id]  AND N.[Deleted] = 0
		LEFT JOIN [hierarchy].[NodeVersion] AS NV ON N.[CurrentNodeVersionId] = NV.[Id]  AND N.[Deleted] = 0
		LEFT JOIN [hierarchy].[CatalogueNodeVersion] AS CNV 
		ON NV.[Id] = CNV.[NodeVersionId] AND CNV.[Deleted] = 0
		INNER JOIN resources.Resource R on RR.ResourceId = R.id 
		INNER JOIN [resources].[ResourceVersion] AS RV ON  RV.Id = R.CurrentResourceVersionId AND RV.deleted =0
		LEFT JOIN resources.ResourceVersionRatingSummary AS RVS On r.CurrentResourceVersionId = rvs.ResourceVersionId
	    INNER JOIN @OriginalResourceReferenceIds ids
        ON ids.Id = rr.OriginalResourceReferenceId
		WHERE RR.[Deleted] = 0 AND R.deleted =0 
		AND (N.[NodeTypeId] <> 4 OR (N.[NodeTypeId] IS NULL))
END