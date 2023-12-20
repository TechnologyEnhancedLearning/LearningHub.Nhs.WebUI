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

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[NodeType] WHERE Id = 4)
BEGIN
	INSERT  [hierarchy].[NodeType] ([Id], [Name], [Description], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted]) VALUES (4, N'External Organisation', N'External Organisation', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
END

IF NOT EXISTS (SELECT 1 FROM [hierarchy].[Node] WHERE [NodeTypeId] = 4)
BEGIN
	INSERT  [hierarchy].[Node] ([NodeTypeId], [Name], [Description], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted]) VALUES (4, N'ESR', N'ESR', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
	DECLARE @NodeId1 INT
	SELECT @NodeId1 = @@IDENTITY
	INSERT  [hierarchy].[NodePath] ( [NodeId], [NodePath],[IsActive], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted]) VALUES ( @NodeId1, @NodeId1, 1, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
	INSERT  [hub].[ExternalOrganisation] ([Id], [NodeId], [Name], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted]) VALUES (1, @NodeId1, N'ESR', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
END


IF NOT EXISTS (SELECT 1 FROM [resources].[EsrLinkType])
BEGIN
	INSERT INTO [resources].[EsrLinkTYpe] ([Name], [Description], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted])	
	VALUES
	(N'NotAvailable', N'Not Available', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
	(N'CreatedUserOnly', N'Available only to created user', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
	(N'CreatedUserAndOtherEditors', N'Available to created user and other editors', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0),
	(N'EveryOne', N'Available to everyone', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
END

-- [resources].[ResourceTypeValidationRule]
IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 1)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (1,6,'Scorm_ManifestPresent',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 2)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (2,6,'Scorm_QuickLinkIdValid',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 3)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (3,6,'Scorm_CatalogEntryValid',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END		

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 4)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (4,6,'Scorm_SchemaVersionValid',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END	

IF NOT EXISTS(SELECT 'X' FROM [resources].[ResourceTypeValidationRule]  WHERE Id = 5)
BEGIN
	INSERT INTO [resources].[ResourceTypeValidationRule] ([Id],[ResourceTypeId],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (5,6,'Scorm_ManifestFluentValidatorValid',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END	