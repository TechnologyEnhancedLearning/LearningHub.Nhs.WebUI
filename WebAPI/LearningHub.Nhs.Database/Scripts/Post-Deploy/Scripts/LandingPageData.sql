

-- Add the initial information page record
IF NOT EXISTS (SELECT 1 FROM [content].[AssetPosition])
BEGIN
    INSERT INTO [content].[AssetPosition] (Id, [Position], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'Top', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    INSERT INTO [content].[AssetPosition] (Id, [Position], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2, 'Center', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    INSERT INTO [content].[AssetPosition] (Id, [Position], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (3, 'Bottom', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END


IF NOT EXISTS (SELECT 1 FROM [content].[PageSectionStatus])
BEGIN
    INSERT INTO [content].[PageSectionStatus] (Id, [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'Draft', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    INSERT INTO [content].[PageSectionStatus] (Id, [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2, 'Processing', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    INSERT INTO [content].[PageSectionStatus] (Id, [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (3, 'Processing Failed', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    INSERT INTO [content].[PageSectionStatus] (Id, [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (4, 'Processed', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    INSERT INTO [content].[PageSectionStatus] (Id, [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (5, 'Live', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [content].[SectionLayoutType])
BEGIN
    INSERT INTO [content].[SectionLayoutType] (Id, [Type], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'Left', 'Left', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    INSERT INTO [content].[SectionLayoutType] (Id, [Type], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2, 'Right', 'Right', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [content].[SectionTemplateType])
BEGIN
    INSERT INTO [content].[SectionTemplateType] (Id, [Type], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'Image', 'Image and text', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    INSERT INTO [content].[SectionTemplateType] (Id, [Type], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2, 'Video', 'Video and text', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [content].[Page])
BEGIN
    SET IDENTITY_INSERT  [content].[Page] ON
    INSERT INTO [content].[Page] (Id, [Name], [url], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'Landing Page', '/', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    SET IDENTITY_INSERT  [content].[Page] OFF
END

IF NOT EXISTS (SELECT 1 FROM [content].[PageSection])
BEGIN
    SET IDENTITY_INSERT  [content].[PageSection] ON
    INSERT INTO [content].[PageSection] (Id, [PageId], [SectionTemplateTypeId], [Position], [IsHidden], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 1, 1, 1, 0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    SET IDENTITY_INSERT  [content].[PageSection] OFF
END


IF NOT EXISTS (SELECT 1 FROM [content].[PageSectionDetail])
BEGIN
    SET IDENTITY_INSERT  [content].[PageSectionDetail] ON
    INSERT INTO [content].[PageSectionDetail] ([Id],[PageSectionId],[PageSectionStatusId],[SectionLayoutTypeId],[Title],[AssetPositionId],[BackgroundColour],[TextColour],[HyperLinkColour],[Description],[Deleted], CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 1, 1, 1, 'Section One', 2, NULL, NULL, NULL, NULL, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
    SET IDENTITY_INSERT  [content].[PageSectionDetail] OFF
END

UPDATE [external].[ExternalSystemDeepLink]
SET [DeepLink] = [DeepLink] + 'HomepageWithAuthentication'
WHERE  [DeepLink] in ('https://webui-dev.test-learninghub.org.uk/', 'https://webui-test.test-learninghub.org.uk/', 'https://learninghub.nhs.uk/')

