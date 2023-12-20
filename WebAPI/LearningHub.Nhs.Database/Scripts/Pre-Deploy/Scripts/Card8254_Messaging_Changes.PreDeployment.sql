/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Killian davies 24.04.21
--------------------------------------------------------------------------------------
*/
IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'UserDismissed'
          AND Object_ID = Object_ID(N'hierarchy.CatalogueAccessRequest'))
BEGIN
	ALTER TABLE [hierarchy].[CatalogueAccessRequest] DROP CONSTRAINT [DF__Catalogue__UserD__72E607DB]
    ALTER TABLE hierarchy.CatalogueAccessRequest DROP COLUMN UserDismissed
END
GO