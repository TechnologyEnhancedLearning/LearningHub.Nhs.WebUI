/*
	Adds the RecordReference and ScormEsrLinkUrl columns to the MigrationInputRecord table
*/

	-- Add new column and temp columns for those appearing after it in the table (allowing NULLs)
	ALTER TABLE [migrations].[MigrationInputRecord]
	ADD [RecordReference] [nvarchar](255) NULL,
		[RecordTitle_temp] [nvarchar](255) NULL,
		[ScormEsrLinkUrl] [nvarchar](4000) NULL,
		[ValidationErrors_temp] [nvarchar](max) NULL,
		[ValidationWarnings_temp] [nvarchar](max) NULL,
		[ExceptionDetails_temp] [nvarchar](max) NULL,
		[ResourceVersionId_temp] [int] NULL,
		[Deleted_temp] [bit] NULL,
		[CreateUserId_temp] [int] NULL,
		[CreateDate_temp] [datetimeoffset](7) NULL,
		[AmendUserId_temp] [int] NULL,
		[AmendDate_temp] [datetimeoffset](7) NULL;

	-- Copy data to new audit columns
	UPDATE [migrations].[MigrationInputRecord]
	SET	[RecordTitle_temp] = [RecordTitle],
		[ValidationErrors_temp] = [ValidationErrors],
		[ValidationWarnings_temp] = [ValidationWarnings],
		[ExceptionDetails_temp] = [ExceptionDetails],
		[ResourceVersionId_temp] = [ResourceVersionId],
		[Deleted_temp] = [Deleted],
		[CreateUserId_temp] = [CreateUserId],
		[CreateDate_temp] = [CreateDate],
		[AmendUserId_temp] = [AmendUserId],
		[AmendDate_temp] = [AmendDate]
	
	-- Set new audit columns to NOT NULL
	ALTER TABLE [migrations].[MigrationInputRecord] ALTER COLUMN [Deleted_temp] [bit] NOT NULL;
	ALTER TABLE [migrations].[MigrationInputRecord] ALTER COLUMN [CreateUserId_temp] [int] NOT NULL;
	ALTER TABLE [migrations].[MigrationInputRecord] ALTER COLUMN [CreateDate_temp] [datetimeoffset](7) NOT NULL;
	ALTER TABLE [migrations].[MigrationInputRecord] ALTER COLUMN [AmendUserId_temp] [int] NOT NULL;
	ALTER TABLE [migrations].[MigrationInputRecord] ALTER COLUMN [AmendDate_temp] [datetimeoffset](7) NOT NULL;

	-- Drop constraint and original audit columns
	ALTER TABLE [migrations].[MigrationInputRecord] DROP CONSTRAINT [FK_MigrationInputRecord_ResourceVersionId]
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [RecordTitle];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [ValidationErrors];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [ValidationWarnings];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [ExceptionDetails];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [ResourceVersionId];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [Deleted];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [CreateUserId];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [CreateDate];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [AmendUserId];
	ALTER TABLE [migrations].[MigrationInputRecord] DROP COLUMN [AmendDate];

	-- Rename to audit columns to the original names
	EXEC sp_rename '[migrations].[MigrationInputRecord].RecordTitle_temp', 'RecordTitle', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].ValidationErrors_temp', 'ValidationErrors', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].ValidationWarnings_temp', 'ValidationWarnings', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].ExceptionDetails_temp', 'ExceptionDetails', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].ResourceVersionId_temp', 'ResourceVersionId', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].Deleted_temp', 'Deleted', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].CreateUserId_temp', 'CreateUserId', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].CreateDate_temp', 'CreateDate', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].AmendUserId_temp', 'AmendUserId', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationInputRecord].AmendDate_temp', 'AmendDate', 'COLUMN';

	-- Recreate constraint
	ALTER TABLE [migrations].[MigrationInputRecord]  WITH CHECK ADD  CONSTRAINT [FK_MigrationInputRecord_ResourceVersionId] FOREIGN KEY([ResourceVersionId])
	REFERENCES [resources].[ResourceVersion] ([Id])

	ALTER TABLE [migrations].[MigrationInputRecord] CHECK CONSTRAINT [FK_MigrationInputRecord_ResourceVersionId]
	

/*
	Makes the DestinationNodeId column on the Migration table nullable.
*/

ALTER TABLE [migrations].[Migration] ALTER COLUMN [DestinationNodeId] INT NULL


/*
	Adds the DefaultEsrLinkTypeId column to the MigrationSource table
*/

	-- Add new column and temp columns for those appearing after it in the table (allowing NULLs)
	ALTER TABLE [migrations].[MigrationSource]
	ADD [DefaultEsrLinkTypeId] [int] NULL,
		[Deleted_temp] [bit] NULL,
		[CreateUserId_temp] [int] NULL,
		[CreateDate_temp] [datetimeoffset](7) NULL,
		[AmendUserId_temp] [int] NULL,
		[AmendDate_temp] [datetimeoffset](7) NULL;

	-- Copy data to new audit columns
	UPDATE [migrations].[MigrationSource]
	SET	[Deleted_temp] = [Deleted],
		[CreateUserId_temp] = [CreateUserId],
		[CreateDate_temp] = [CreateDate],
		[AmendUserId_temp] = [AmendUserId],
		[AmendDate_temp] = [AmendDate]
	
	-- Set new audit columns to NOT NULL
	ALTER TABLE [migrations].[MigrationSource] ALTER COLUMN [Deleted_temp] [bit] NOT NULL;
	ALTER TABLE [migrations].[MigrationSource] ALTER COLUMN [CreateUserId_temp] [int] NOT NULL;
	ALTER TABLE [migrations].[MigrationSource] ALTER COLUMN [CreateDate_temp] [datetimeoffset](7) NOT NULL;
	ALTER TABLE [migrations].[MigrationSource] ALTER COLUMN [AmendUserId_temp] [int] NOT NULL;
	ALTER TABLE [migrations].[MigrationSource] ALTER COLUMN [AmendDate_temp] [datetimeoffset](7) NOT NULL;

	-- Drop original audit columns
	ALTER TABLE [migrations].[MigrationSource] DROP COLUMN [Deleted];
	ALTER TABLE [migrations].[MigrationSource] DROP COLUMN [CreateUserId];
	ALTER TABLE [migrations].[MigrationSource] DROP COLUMN [CreateDate];
	ALTER TABLE [migrations].[MigrationSource] DROP COLUMN [AmendUserId];
	ALTER TABLE [migrations].[MigrationSource] DROP COLUMN [AmendDate];

	-- Rename to audit columns to the original names
	EXEC sp_rename '[migrations].[MigrationSource].Deleted_temp', 'Deleted', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationSource].CreateUserId_temp', 'CreateUserId', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationSource].CreateDate_temp', 'CreateDate', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationSource].AmendUserId_temp', 'AmendUserId', 'COLUMN';
	EXEC sp_rename '[migrations].[MigrationSource].AmendDate_temp', 'AmendDate', 'COLUMN';

	-- Set the value for the existing eLR migration record (ID=1)
	UPDATE [migrations].[MigrationSource] SET DefaultEsrLinkTypeId = 4 WHERE ID = 1

/*
	Adds the DefaultEsrLinkTypeId column to the Migration table
*/

	-- Add new column and temp columns for those appearing after it in the table (allowing NULLs)
	ALTER TABLE [migrations].[Migration]
	ADD [DefaultEsrLinkTypeId] [int] NULL,
		[Deleted_temp] [bit] NULL,
		[CreateUserId_temp] [int] NULL,
		[CreateDate_temp] [datetimeoffset](7) NULL,
		[AmendUserId_temp] [int] NULL,
		[AmendDate_temp] [datetimeoffset](7) NULL;

	-- Copy data to new audit columns
	UPDATE [migrations].[Migration]
	SET	[Deleted_temp] = [Deleted],
		[CreateUserId_temp] = [CreateUserId],
		[CreateDate_temp] = [CreateDate],
		[AmendUserId_temp] = [AmendUserId],
		[AmendDate_temp] = [AmendDate]
	
	-- Set new audit columns to NOT NULL
	ALTER TABLE [migrations].[Migration] ALTER COLUMN [Deleted_temp] [bit] NOT NULL;
	ALTER TABLE [migrations].[Migration] ALTER COLUMN [CreateUserId_temp] [int] NOT NULL;
	ALTER TABLE [migrations].[Migration] ALTER COLUMN [CreateDate_temp] [datetimeoffset](7) NOT NULL;
	ALTER TABLE [migrations].[Migration] ALTER COLUMN [AmendUserId_temp] [int] NOT NULL;
	ALTER TABLE [migrations].[Migration] ALTER COLUMN [AmendDate_temp] [datetimeoffset](7) NOT NULL;

	-- Drop original audit columns
	ALTER TABLE [migrations].[Migration] DROP COLUMN [Deleted];
	ALTER TABLE [migrations].[Migration] DROP COLUMN [CreateUserId];
	ALTER TABLE [migrations].[Migration] DROP COLUMN [CreateDate];
	ALTER TABLE [migrations].[Migration] DROP COLUMN [AmendUserId];
	ALTER TABLE [migrations].[Migration] DROP COLUMN [AmendDate];

	-- Rename to audit columns to the original names
	EXEC sp_rename '[migrations].[Migration].Deleted_temp', 'Deleted', 'COLUMN';
	EXEC sp_rename '[migrations].[Migration].CreateUserId_temp', 'CreateUserId', 'COLUMN';
	EXEC sp_rename '[migrations].[Migration].CreateDate_temp', 'CreateDate', 'COLUMN';
	EXEC sp_rename '[migrations].[Migration].AmendUserId_temp', 'AmendUserId', 'COLUMN';
	EXEC sp_rename '[migrations].[Migration].AmendDate_temp', 'AmendDate', 'COLUMN';