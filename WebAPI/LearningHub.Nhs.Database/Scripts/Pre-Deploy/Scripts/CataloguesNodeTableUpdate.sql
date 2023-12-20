/*
	Adds the 'Hidden column to the Node table
*/

    ALTER TABLE [hierarchy].[Node] SET (SYSTEM_VERSIONING = OFF);

	ALTER TABLE [hierarchy].[Node] DROP PERIOD FOR SYSTEM_TIME;

	DROP TABLE [hierarchy].[NodeHistory];

	ALTER TABLE [hierarchy].[Node] DROP COLUMN [VersionStartTime];
	ALTER TABLE [hierarchy].[Node] DROP COLUMN [VersionEndTime];
	GO

	-- Add new column and new audit columns (allowing NULLs)
	ALTER TABLE [hierarchy].[Node]
	ADD [Hidden] [bit] NOT NULL DEFAULT 0,
		[Deleted_temp] [bit] NULL,
		[CreateUserId_temp] [int] NULL,
		[CreateDate_temp] [datetimeoffset](7) NULL,
		[AmendUserId_temp] [int] NULL,
		[AmendDate_temp] [datetimeoffset](7) NULL;
	GO

	-- Copy data to new audit columns
	UPDATE [hierarchy].[Node]
	SET	[Deleted_temp] = [Deleted],
		[CreateUserId_temp] = [CreateUserId],
		[CreateDate_temp] = [CreateDate],
		[AmendUserId_temp] = [AmendUserId],
		[AmendDate_temp] = [AmendDate]
	
	-- Set new audit columns to NOT NULL
	ALTER TABLE [hierarchy].[Node] ALTER COLUMN [Deleted_temp] [bit] NOT NULL;
	ALTER TABLE [hierarchy].[Node] ALTER COLUMN [CreateUserId_temp] [int] NOT NULL;
	ALTER TABLE [hierarchy].[Node] ALTER COLUMN [CreateDate_temp] [datetimeoffset](7) NOT NULL;
	ALTER TABLE [hierarchy].[Node] ALTER COLUMN [AmendUserId_temp] [int] NOT NULL;
	ALTER TABLE [hierarchy].[Node] ALTER COLUMN [AmendDate_temp] [datetimeoffset](7) NOT NULL;

	-- Drop constraint and original audit columns
	ALTER TABLE [hierarchy].[Node] DROP COLUMN [Deleted];
	ALTER TABLE [hierarchy].[Node] DROP COLUMN [CreateUserId];
	ALTER TABLE [hierarchy].[Node] DROP COLUMN [CreateDate];
	ALTER TABLE [hierarchy].[Node] DROP COLUMN [AmendUserId];
	ALTER TABLE [hierarchy].[Node] DROP COLUMN [AmendDate];

	-- Rename to audit columns to the original names
	EXEC sp_rename '[hierarchy].[Node].Deleted_temp', 'Deleted', 'COLUMN';
	EXEC sp_rename '[hierarchy].[Node].CreateUserId_temp', 'CreateUserId', 'COLUMN';
	EXEC sp_rename '[hierarchy].[Node].CreateDate_temp', 'CreateDate', 'COLUMN';
	EXEC sp_rename '[hierarchy].[Node].AmendUserId_temp', 'AmendUserId', 'COLUMN';
	EXEC sp_rename '[hierarchy].[Node].AmendDate_temp', 'AmendDate', 'COLUMN';

	-- Add new column and new audit columns (allowing NULLs)
	ALTER TABLE [hierarchy].[Node]
	ADD [VersionStartTime]     DATETIME2 (7) NULL,
		[VersionEndTime]       DATETIME2 (7) NULL;

	GO	
	
	UPDATE [hierarchy].[Node]
	SET	[VersionStartTime] = DATEADD(day,-1,SYSDATETIME()),
		[VersionEndTime] = '9999-12-31 23:59:59.9999999'

	ALTER TABLE [hierarchy].[Node] ALTER COLUMN [VersionStartTime] DATETIME2 (7) NOT NULL;
	ALTER TABLE [hierarchy].[Node] ALTER COLUMN [VersionEndTime] DATETIME2 (7) NOT NULL;
	GO

	ALTER TABLE [hierarchy].[Node] ADD PERIOD FOR SYSTEM_TIME (VersionStartTime, VersionEndTime)


    ALTER TABLE [hierarchy].[Node]
	SET (SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hierarchy].[NodeHistory]) );


