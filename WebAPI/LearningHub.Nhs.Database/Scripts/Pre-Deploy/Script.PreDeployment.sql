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

-- TD-2902 - IF the [Title] column in [Content].[PageSectionDetail] has not been removed then raise error, script needs to be run manually
IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'resources'
                  AND TABLE_NAME = 'ResourceReferenceEvent'))
BEGIN
    RAISERROR (N'TD-2902 Add resource types to Content Server.sql must be run manually before release.', 16, 127) WITH NOWAIT
END
GO

--TD-4016-Remove character limit from catalogue description
ALTER TABLE hierarchy.CatalogueNodeVersion ALTER COLUMN Description nvarchar(max);
GO