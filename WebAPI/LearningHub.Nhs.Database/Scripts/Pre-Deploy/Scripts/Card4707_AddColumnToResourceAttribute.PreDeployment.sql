/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Robert Smith - 22 Feb 2021
	Card 4707 - Add new Value column to resource.ResourceAttribute table. 
    New manual script to avoid DACPAC build errors.
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Value'
          AND Object_ID = Object_ID(N'resources.ResourceAttribute'))
BEGIN
    -- Delete existing columns. Table not yet used, so no need to store audit data in temp columns.
    ALTER TABLE [resources].[ResourceAttribute] DROP COLUMN [Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]

    -- Add new Value column and replace audit columns at the end.
    ALTER TABLE [resources].[ResourceAttribute]
    ADD [Value] [nvarchar](255) NOT NULL,
	    [Deleted] [bit] NOT NULL,
	    [CreateUserId] [int] NOT NULL,
	    [CreateDate] [datetimeoffset](7) NOT NULL,
	    [AmendUserId] [int] NOT NULL,
	    [AmendDate] [datetimeoffset](7) NOT NULL;

END
GO
