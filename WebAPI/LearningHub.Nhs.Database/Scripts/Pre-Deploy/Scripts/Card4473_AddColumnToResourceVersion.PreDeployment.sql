/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Dave Brown - 17 Jun 2020
	Card 4473 - Adding the SensitiveContent column to the ResourceVersion table
				whilst keeping the audit columns at the end of the table.
--------------------------------------------------------------------------------------
*/

-- Add new column and new audit columns (allowing NULLs)
ALTER TABLE resources.ResourceVersion
ADD [SensitiveContent] [bit] NOT NULL DEFAULT 0,
	[Deleted_temp] [bit] NULL,
	[CreateUserId_temp] [int] NULL,
	[CreateDate_temp] [datetimeoffset](7) NULL,
	[AmendUserId_temp] [int] NULL,
	[AmendDate_temp] [datetimeoffset](7) NULL;
GO

-- Add copy data to new audit columns
UPDATE resources.ResourceVersion
SET	[Deleted_temp] = [Deleted],
	[CreateUserId_temp] = [CreateUserId],
	[CreateDate_temp] = [CreateDate],
	[AmendUserId_temp] = [AmendUserId],
	[AmendDate_temp] = [AmendDate]
GO

-- Set new audit columns to NOT NULL
ALTER TABLE resources.ResourceVersion ALTER COLUMN [Deleted_temp] [bit] NOT NULL;
ALTER TABLE resources.ResourceVersion ALTER COLUMN [CreateUserId_temp] [int] NOT NULL;
ALTER TABLE resources.ResourceVersion ALTER COLUMN [CreateDate_temp] [datetimeoffset](7) NOT NULL;
ALTER TABLE resources.ResourceVersion ALTER COLUMN [AmendUserId_temp] [int] NOT NULL;
ALTER TABLE resources.ResourceVersion ALTER COLUMN [AmendDate_temp] [datetimeoffset](7) NOT NULL;
GO

-- Drop constraint and original audit columns
ALTER TABLE resources.ResourceVersion DROP CONSTRAINT [FK_ResourceVersion_CreateUser];
ALTER TABLE resources.ResourceVersion DROP COLUMN [Deleted];
ALTER TABLE resources.ResourceVersion DROP COLUMN [CreateUserId];
ALTER TABLE resources.ResourceVersion DROP COLUMN [CreateDate];
ALTER TABLE resources.ResourceVersion DROP COLUMN [AmendUserId];
ALTER TABLE resources.ResourceVersion DROP COLUMN [AmendDate];
GO

-- Rename to audit columns to the original names
EXEC sp_rename 'resources.ResourceVersion.Deleted_temp', 'Deleted', 'COLUMN';
EXEC sp_rename 'resources.ResourceVersion.CreateUserId_temp', 'CreateUserId', 'COLUMN';
EXEC sp_rename 'resources.ResourceVersion.CreateDate_temp', 'CreateDate', 'COLUMN';
EXEC sp_rename 'resources.ResourceVersion.AmendUserId_temp', 'AmendUserId', 'COLUMN';
EXEC sp_rename 'resources.ResourceVersion.AmendDate_temp', 'AmendDate', 'COLUMN';
GO

-- Re Add the constraint
ALTER TABLE [resources].[ResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersion_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO
