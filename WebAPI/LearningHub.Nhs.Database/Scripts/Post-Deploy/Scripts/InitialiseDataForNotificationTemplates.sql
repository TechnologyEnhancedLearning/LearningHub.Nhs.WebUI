/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT 1 FROM [hub].[NotificationTemplate] WHERE Title = 'CatalogueAccessRequestSuccess')
BEGIN
    INSERT INTO [hub].[NotificationTemplate] (Id, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'CatalogueAccessRequestSuccess', 'Access request approved', 'Your request to access the <a href="[CatalogueUrl]" target="_blank">[CatalogueName]</a> catalogue has been approved.', '[CatalogueName][CatalogueUrl]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [hub].[NotificationTemplate] WHERE Title = 'CatalogueAccessRequestFailure')
BEGIN
    INSERT INTO [hub].[NotificationTemplate] (Id, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2, 'CatalogueAccessRequestFailure', 'Access request not approved', 'The catalogue administrator did not approve your request to access the <a href="[CatalogueUrl]" target="_blank">[CatalogueName]<a/> catalogue. The following reason was given:<br/><br/><p>[RejectionReason]</p>', '[RejectionReason][CatalogueName][CatalogueUrl]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

--IF NOT EXISTS (SELECT 1 FROM [hub].[NotificationTemplate] WHERE Title = 'RoleRemovedForCatalogue')
--BEGIN
--    INSERT INTO [hub].[NotificationTemplate] (Id, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
--    VALUES (3, 'RoleRemovedForCatalogue', 'Change of permissions', 'You no longer have the role of <b>[RoleName]</b> in the catalogue <b>[CatalogueName]</b>', '[RoleName][CatalogueName]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
--END

IF NOT EXISTS (SELECT 1 FROM [hub].[NotificationTemplate] WHERE Title = 'CatalogueEditorAdded')
BEGIN
    INSERT INTO [hub].[NotificationTemplate] (Id, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (3, 'CatalogueEditorAdded', 'What you can do in the Learning Hub has changed', 'You have been assigned the role of catalogue editor for <a href="[CatalogueUrl]" target="_blank">[CatalogueName]</a>. This means that only you and other assigned editors can manage the catalogue. If you have any questions about this, please contact the <a href="[SupportUrl]" target="_blank">support team</a>.', '[SupportUrl][CatalogueName][CatalogueUrl]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [hub].[NotificationTemplate] WHERE Title = 'CatalogueEditorRemoved')
BEGIN
    INSERT INTO [hub].[NotificationTemplate] (Id, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (4, 'CatalogueEditorRemoved', 'What you can do in the Learning Hub has changed', 'You no longer have the role of catalogue editor for <a href="[CatalogueUrl]" target="_blank">[CatalogueName]</a>. This means that you cannot manage the catalogue.
You can continue to search for, access and contribute resources in the Learning Hub. 
If you have any questions about this, please contact the <a href="[SupportUrl]" target="_blank">support team</a>.', '[SupportUrl][CatalogueName][CatalogueUrl]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END