--TD-4016-Remove character limit from catalogue description

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Description'
          AND Object_ID = Object_ID(N'hierarchy.CatalogueNodeVersion'))
BEGIN
    ALTER TABLE hierarchy.CatalogueNodeVersion ALTER COLUMN Description nvarchar(max) null;
END