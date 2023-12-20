-------------------------------------------------------------------------------
-- Author       Jignesh Jethwani
-- Created      21-07-2023
-- Purpose      Gets the resource version number from major and minor version
--
-- Modification History
-- 21-07-2023  Jignesh Jethwani - Initial Revision

CREATE FUNCTION [activity].[CalculateResourceVersionNumber](@MajorVersion INT, @MinorVersion INT)  
RETURNS BIGINT  
AS   
BEGIN  
  DECLARE @VersionNumber AS BIGINT;    

  SET @VersionNumber = CAST(CAST(@MajorVersion AS VARCHAR(10)) + CAST(@MinorVersion AS VARCHAR(10)) as BIGINT)  

  RETURN @VersionNumber;  
END  