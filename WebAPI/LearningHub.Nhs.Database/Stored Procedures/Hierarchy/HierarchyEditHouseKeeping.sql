-------------------------------------------------------------------------------
-- Author       Dave Brown
-- Created      08 Dec 2022
-- Purpose      Removes old temporary Hierarchy Edit related data before new Catalogue edit.
--
-- Modification History
--
-- 08 Dec 2022  DB	Initial Revision.
-- 07-05-2024   DB	Change input parameter to NodePathId to match [hierarchy].[HierarchyEditCreate].
-------------------------------------------------------------------------------
CREATE PROCEDURE [hierarchy].[HierarchyEditHouseKeeping]
(
	@RootNodePathId int
)

AS

BEGIN

	BEGIN TRY

		DELETE FROM hed
		FROM [hierarchy].[HierarchyEditDetail] hed
		INNER JOIN [hierarchy].[HierarchyEdit] he ON hed.HierarchyEditId = he.Id
		WHERE	he.RootNodePathId = @RootNodePathId
		AND he.HierarchyEditStatusId in (2 /* Published */, 3 /* Discarded */, 6 /* Failed to Publish */)


		DELETE FROM henrl
		FROM [hierarchy].[HierarchyEditNodeResourceLookup] henrl
		INNER JOIN [hierarchy].[HierarchyEdit] he ON henrl.HierarchyEditId = he.Id
		WHERE	he.RootNodePathId = @RootNodePathId
		AND he.HierarchyEditStatusId in (2 /* Published */, 3 /* Discarded */, 6 /* Failed to Publish */)

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