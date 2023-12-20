/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Dave Brown - 05 Jun 2020
	Card 5766 - Update Contribute copy on Author/Organisation section
	    RoleOrOrganisation column to be changes to Organisation,
        additional changes to be made by the release schema update
--------------------------------------------------------------------------------------
*/

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'RoleOrOrganisation'
          AND Object_ID = Object_ID(N'resources.ResourceVersionAuthor'))
BEGIN
    EXEC sp_rename 'resources.ResourceVersionAuthor.RoleOrOrganisation', 'Organisation', 'COLUMN';
END

GO
