/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed before the build script.	
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

-- Content Referencing changes
IF EXISTS (SELECT * from hierarchy.HierarchyEdit WHERE HierarchyEditStatusId = 1)-- Draft
BEGIN
    RAISERROR ('Script ''Content Referencing Initialise.sql'' must be manually run before deploy', 16, 1)
    RETURN
END
GO
