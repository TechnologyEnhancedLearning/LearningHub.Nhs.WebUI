/*
 Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
    Robert Smith - 19 Aug 2020
	Card 6502 - Changes to ScormResourceVersion area:
    - Delete all existing columns from ScormResourceVersion table as not used (apart from ResourceVersionId).
    - Add ZipFileId col and audit cols to ScormResourceVersion table, plus constraints.
--------------------------------------------------------------------------------------
*/

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ScormPackageTypeId'
          AND Object_ID = Object_ID(N'resources.ScormResourceVersion'))
BEGIN
    -- Delete existing columns. Table not yet used, so no need to store audit data in temp columns.
    ALTER TABLE [resources].[ScormResourceVersion] DROP CONSTRAINT [FK_ScormResourceVersion_ScormPackageType]
    ALTER TABLE [resources].[ScormResourceVersion] DROP COLUMN [ScormPackageTypeId],[DevelopmentId],[Title],[Duration],[ManifestURL],[QuicklinkId],[CatalogEntry],
    [Copyright],[ResourceIdentifier],[ItemIdentifier],[TemplateVersion],[PopupWidth],[PopupHeight],[PopupAdjustable],[MasteryScore],[Folder],[FileContentType]
    ,[StatusCompleteOnLaunch],[AiccEnabled],[AiccEnabledDate],[FullScreenInd],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate]

    -- Add new ZipFileId column and replace audit columns at the end.
    ALTER TABLE resources.ScormResourceVersion
    ADD [ZipFileId] [int] NOT NULL,
	    [Deleted] [bit] NOT NULL,
	    [CreateUserId] [int] NOT NULL,
	    [CreateDate] [datetimeoffset](7) NOT NULL,
	    [AmendUserId] [int] NOT NULL,
	    [AmendDate] [datetimeoffset](7) NOT NULL;

    -- Add constraints
    ALTER TABLE [resources].[ScormResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ScormResourceVersion_File] FOREIGN KEY([ZipFileId]) REFERENCES [resources].[File] ([Id])
    ALTER TABLE [resources].[ScormResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ScormResourceVersion_CreateUser] FOREIGN KEY([CreateUserId]) REFERENCES [hub].[User] ([Id])
    ALTER TABLE [resources].[ScormResourceVersion]  WITH CHECK ADD  CONSTRAINT [FK_ScormResourceVersion_AmendUser] FOREIGN KEY([AmendUserId]) REFERENCES [hub].[User] ([Id])

END
GO
