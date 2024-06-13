IF EXISTS (SELECT * from hierarchy.HierarchyEdit WHERE HierarchyEditStatusId = 1)-- Draft
BEGIN
    RAISERROR ('Cannot delete hierarchy.RootNodeId column with an edit in Draft status', 16, 1)
    RETURN
END

-- delete column RootNodeId from hierarchy.HierarchyEdit if it exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HierarchyEdit' AND COLUMN_NAME = 'RootNodeId')
BEGIN
    ALTER TABLE hierarchy.HierarchyEdit DROP CONSTRAINT FK_HierarchyEdit_Node
    ALTER TABLE hierarchy.HierarchyEdit DROP COLUMN RootNodeId
END
GO

-- Add record to table hierarchy.HierarchyEditDetailOperation if it doesn't already exist:
IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailOperation] WHERE [Id] = 4)
BEGIN
    INSERT INTO [hierarchy].[HierarchyEditDetailOperation] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
    VALUES (4,N'Add Reference',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END
GO

-- Add the IsActive column to the ResourceReference table if it doesn't already exist
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'ResourceReference' AND COLUMN_NAME = 'IsActive')
BEGIN
    IF EXISTS (SELECT * FROM information_schema.table_constraints WHERE constraint_name = 'FK_ResourceReferenceEvent_ResourceReference')
    BEGIN
        ALTER TABLE [resources].[ResourceReferenceEvent] DROP CONSTRAINT [FK_ResourceReferenceEvent_ResourceReference];
    END
    IF EXISTS (SELECT * FROM information_schema.table_constraints WHERE constraint_name = 'FK_ExternalReference_ResourceReference')
    BEGIN
        ALTER TABLE [resources].[ExternalReference] DROP CONSTRAINT [FK_ExternalReference_ResourceReference];
    END
    IF EXISTS (SELECT * FROM information_schema.table_constraints WHERE constraint_name = 'FK_ResourceReferenceDisplayVersion_ResourceReference')
    BEGIN
        ALTER TABLE [resources].[ResourceReferenceDisplayVersion] DROP CONSTRAINT [FK_ResourceReferenceDisplayVersion_ResourceReference];
    END
    IF EXISTS (SELECT * FROM information_schema.table_constraints WHERE constraint_name = 'FK_OriginalResourceReference_ResourceReference')
    BEGIN
        ALTER TABLE [resources].[ResourceReference] DROP CONSTRAINT [FK_OriginalResourceReference_ResourceReference];
    END
    IF EXISTS (SELECT * FROM information_schema.table_constraints WHERE constraint_name = 'FK_ResourceReference_NodePath')
    BEGIN
        ALTER TABLE [resources].[ResourceReference] DROP CONSTRAINT [FK_ResourceReference_NodePath];
    END
    IF EXISTS (SELECT * FROM information_schema.table_constraints WHERE constraint_name = 'FK_ResourceReference_Resource')
    BEGIN
        ALTER TABLE [resources].[ResourceReference] DROP CONSTRAINT [FK_ResourceReference_Resource];
    END
    IF EXISTS (SELECT * FROM information_schema.table_constraints WHERE constraint_name = 'FK_UserBookmark_ResourceReference')
    BEGIN
        ALTER TABLE [hub].[UserBookmark] DROP CONSTRAINT [FK_UserBookmark_ResourceReference];
    END
    IF EXISTS (SELECT * FROM information_schema.table_constraints WHERE constraint_name = 'FK_ScormResourceReferenceEvent_ResourceReference')
    BEGIN
        ALTER TABLE [resources].[ScormResourceReferenceEvent] DROP CONSTRAINT [FK_ScormResourceReferenceEvent_ResourceReference];
    END

    CREATE TABLE [resources].[tmp_ms_xx_ResourceReference] (
        [Id]                          INT                IDENTITY (1, 1) NOT NULL,
        [ResourceId]                  INT                NOT NULL,
        [NodePathId]                  INT                NULL,
        [OriginalResourceReferenceId] INT                NULL,
        [IsActive]                    BIT                DEFAULT 1 NOT NULL,
        [Deleted]                     BIT                NOT NULL,
        [CreateUserId]                INT                NOT NULL,
        [CreateDate]                  DATETIMEOFFSET (7) NOT NULL,
        [AmendUserId]                 INT                NOT NULL,
        [AmendDate]                   DATETIMEOFFSET (7) NOT NULL,
        CONSTRAINT [tmp_ms_xx_constraint_PK_Resources_ResourceReference1] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    IF EXISTS (SELECT TOP 1 1 
               FROM   [resources].[ResourceReference])
        BEGIN
            SET IDENTITY_INSERT [resources].[tmp_ms_xx_ResourceReference] ON;
            INSERT INTO [resources].[tmp_ms_xx_ResourceReference] ([Id], [ResourceId], [NodePathId], [OriginalResourceReferenceId], [IsActive], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
            SELECT   [Id],
                     [ResourceId],
                     [NodePathId],
                     [OriginalResourceReferenceId],
				     1 AS [IsActive],
                     [Deleted],
                     [CreateUserId],
                     [CreateDate],
                     [AmendUserId],
                     [AmendDate]
            FROM     [resources].[ResourceReference]
            ORDER BY [Id] ASC;
            SET IDENTITY_INSERT [resources].[tmp_ms_xx_ResourceReference] OFF;
        END

    DROP TABLE [resources].[ResourceReference];

    EXECUTE sp_rename N'[resources].[tmp_ms_xx_ResourceReference]', N'ResourceReference';

    EXECUTE sp_rename N'[resources].[tmp_ms_xx_constraint_PK_Resources_ResourceReference1]', N'PK_Resources_ResourceReference', N'OBJECT';

    /* NOTE */
    -- The removed constraints will be re-added.
    -- They will automatically be re-added by the deployment release script.
    -- NOTE: When running locally the Database Schema Comapre tool should be used to re-add the constraints.
END
GO