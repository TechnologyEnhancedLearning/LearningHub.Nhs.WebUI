--TD-4016-Remove character limit from catalogue description
ALTER TABLE hierarchy.CatalogueNodeVersion ALTER COLUMN Description nvarchar(max) NULL;
GO