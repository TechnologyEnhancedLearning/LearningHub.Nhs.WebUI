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

IF NOT EXISTS (SELECT * FROM [hub].[Role] WHERE id = 4)
BEGIN
	SET IDENTITY_INSERT [hub].[Role] ON
	INSERT INTO [hub].[Role] ([Id], [Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (4, 'Debugger','Debugger Role',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
	SET IDENTITY_INSERT [hub].[Role] OFF

	INSERT INTO [hub].[RoleUserGroup]([RoleId],[UserGroupId],[ScopeId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(4,2,NULL,0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END

IF NOT EXISTS (SELECT * FROM [hub].[Permission] WHERE [Code] = 'Scorm_Trace_Window')
BEGIN
	DECLARE @PermissionId int

	INSERT INTO [hub].[Permission] ([Code],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES ('Scorm_Trace_Window','Access to Scorm Trace window','Access to Scorm Trace window',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
	SET @PermissionId = SCOPE_IDENTITY()

	INSERT INTO [hub].[PermissionRole]([PermissionId],[RoleId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES(@PermissionId,4,0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END