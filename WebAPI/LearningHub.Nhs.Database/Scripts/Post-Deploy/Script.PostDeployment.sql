/*
 Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be executed after the build script.	
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

--IF NOT EXISTS (SELECT 1 FROM [resources].[ScormResourceReferenceEventType])
--BEGIN
--	INSERT INTO [resources].[ScormResourceReferenceEventType] ([Name], [Description], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted])	
--	VALUES
--	(N'Status200OK', N'Status200OK', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
--	(N'Status410Gone', N'Status410Gone', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
--	(N'Status403Forbidden', N'Status403Forbidden', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
--	(N'Status404NotFound', N'Status404NotFound', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
--END

IF EXISTS( SELECT 1 FROM [resources].[ResourceVersionKeyword] WHERE Keyword LIKE '%  %')
BEGIN
	UPDATE  [resources].[ResourceVersionKeyword] set 
	Keyword = REPLACE(REPLACE(REPLACE(Keyword, ' ', '<>'), '><', ''),'<>',' ')

	UPDATE  [resources].[ResourceVersionKeyword]  SET Deleted = 1, AmendUserId = 4, AmendDate = SYSDATETIMEOFFSET()
	WHERE Id NOT IN (
		SELECT MAX(Id) 
		FROM  [resources].[ResourceVersionKeyword] 
		WHERE Deleted = 0 
		GROUP BY Keyword, ResourceVersionId
	);
END

IF NOT EXISTS (SELECT 1 FROM [hub].[DetectJsLog])
BEGIN
	INSERT INTO [hub].[DetectJsLog] ([JsEnabledRequest], [JsDisabledRequest], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted])	
	VALUES	(0, 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
END

IF NOT EXISTS (SELECT * FROM [hub].[Role] WHERE id = 7)
BEGIN
	SET IDENTITY_INSERT [hub].[Role] ON
	INSERT INTO [hub].[Role] ([Id], [Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (7, 'Reporter','Reporter Role',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
	SET IDENTITY_INSERT [hub].[Role] OFF

	INSERT INTO [hub].[RoleUserGroup]([RoleId],[UserGroupId],[ScopeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(7,2,NULL,0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END

IF NOT EXISTS (SELECT 1 FROM [resources].[ResourceAccessibility])
BEGIN
	INSERT INTO [resources].[ResourceAccessibility] ([Name], [Description], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted])	
	VALUES
	(N'Public Access', N'Public Access', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
	(N'General Access', N'General Access', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
	(N'Full Access', N'Full Access', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
	(N'None', N'None', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)

	UPDATE [resources].[ResourceVersion]  SET [ResourceAccessibilityId] = 3

	ALTER TABLE [resources].[ResourceVersion] 
	WITH CHECK ADD CONSTRAINT [FK_ResourceVersion_ResourceAccessibility] FOREIGN KEY([ResourceAccessibilityId])
	REFERENCES [resources].[ResourceAccessibility] ([Id])
END

UPDATE [resources].[ResourceVersion] SET CertificateEnabled = 0 WHERE VersionStatusId <> 1 AND CertificateEnabled IS NULL

:r .\Scripts\Bookmark.sql
:r .\Scripts\PopulateEventTypes.sql
:r .\Scripts\InternalSystem.sql
:r .\Scripts\UpdateDataForEmailTemplates.sql
:r .\Scripts\RolePermissionForReleaseOffline.sql
:r .\Scripts\ExternalSystemData.sql
:r .\Scripts\EmailChangeMessageSeed.sql
:r .\Scripts\PopulateProviders.sql
:r .\Scripts\HtmlResourceType.sql
:r .\Scripts\InitialiseDataForEmailTemplates.sql
:r .\Scripts\TD-2929_ActivityStatusUpdates.sql
:r .\Scripts\InitialiseDataForEmailTemplates.sql
:r .\Scripts\AttributeData.sql 
:r .\Scripts\PPSXFileType.sql