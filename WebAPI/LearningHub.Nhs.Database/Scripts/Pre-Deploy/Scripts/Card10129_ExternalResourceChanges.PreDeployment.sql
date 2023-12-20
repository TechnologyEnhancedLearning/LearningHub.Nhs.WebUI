/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Dave Brown - 01 Jun 2021
	Card 10129 - Addition of the active column to the ExternalReference table.
--------------------------------------------------------------------------------------
*/

	-- Add new column and new audit columns (allowing NULLs)
	ALTER TABLE [resources].[ExternalReference]
	ADD [Active] [bit] NULL,
		[Deleted_temp] [bit] NULL,
		[CreateUserId_temp] [int] NULL,
		[CreateDate_temp] [datetimeoffset](7) NULL,
		[AmendUserId_temp] [int] NULL,
		[AmendDate_temp] [datetimeoffset](7) NULL;
	GO

	-- Copy data to new audit columns
	UPDATE [resources].[ExternalReference]
	SET	[Active] = 1,
		[Deleted_temp] = [Deleted],
		[CreateUserId_temp] = [CreateUserId],
		[CreateDate_temp] = [CreateDate],
		[AmendUserId_temp] = [AmendUserId],
		[AmendDate_temp] = [AmendDate]
	
	-- Set new audit columns to NOT NULL
	ALTER TABLE [resources].[ExternalReference] ALTER COLUMN [Active] [bit] NOT NULL;
	ALTER TABLE [resources].[ExternalReference] ALTER COLUMN [Deleted_temp] [bit] NOT NULL;
	ALTER TABLE [resources].[ExternalReference] ALTER COLUMN [CreateUserId_temp] [int] NOT NULL;
	ALTER TABLE [resources].[ExternalReference] ALTER COLUMN [CreateDate_temp] [datetimeoffset](7) NOT NULL;
	ALTER TABLE [resources].[ExternalReference] ALTER COLUMN [AmendUserId_temp] [int] NOT NULL;
	ALTER TABLE [resources].[ExternalReference] ALTER COLUMN [AmendDate_temp] [datetimeoffset](7) NOT NULL;

	-- Drop constraint and original audit columns
	ALTER TABLE [resources].[ExternalReference] DROP COLUMN [Deleted];
	ALTER TABLE [resources].[ExternalReference] DROP COLUMN [CreateUserId];
	ALTER TABLE [resources].[ExternalReference] DROP COLUMN [CreateDate];
	ALTER TABLE [resources].[ExternalReference] DROP COLUMN [AmendUserId];
	ALTER TABLE [resources].[ExternalReference] DROP COLUMN [AmendDate];

	-- Rename to audit columns to the original names
	EXEC sp_rename '[resources].[ExternalReference].Deleted_temp', 'Deleted', 'COLUMN';
	EXEC sp_rename '[resources].[ExternalReference].CreateUserId_temp', 'CreateUserId', 'COLUMN';
	EXEC sp_rename '[resources].[ExternalReference].CreateDate_temp', 'CreateDate', 'COLUMN';
	EXEC sp_rename '[resources].[ExternalReference].AmendUserId_temp', 'AmendUserId', 'COLUMN';
	EXEC sp_rename '[resources].[ExternalReference].AmendDate_temp', 'AmendDate', 'COLUMN';
