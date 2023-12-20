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

-- TD-887 - IF the [Title] column in [Content].[PageSectionDetail] has not been removed then raise error, script needs to be run manually
IF EXISTS(SELECT 1 FROM sys.columns 
        WHERE Name = N'Title'
		AND Object_ID = Object_ID(N'Content.PageSectionDetail'))
BEGIN
    RAISERROR (N'The script .\Scripts\TD-887_PreLoginLandingPageChanges.sql must be run manually before release.', 16, 127) WITH NOWAIT
    --:r .\Scripts\TD-887_PreLoginLandingPageChanges.sql
END
GO