-------------------------------------------------------------------------------
-- Author       Jermey Brockman
-- Created      10-03-2021
-- Purpose      [lakeflowUtilityVersion_1_1] - Data bricks
--
-- Modification History
--
-- 28-01-2026  Jermey Brockman	Initial Revision
-------------------------------------------------------------------------------
CREATE FUNCTION [dbo].[lakeflowUtilityVersion_1_1]()
RETURNS NVARCHAR(10)
AS
BEGIN
    RETURN '1.1';
END