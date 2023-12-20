/*
Pre-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be prepended to the build script.		
 Use SQLCMD syntax to include a file in the pre-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the pre-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF EXISTS(SELECT 1 FROM [hub].[Attribute] WHERE Name = 'Resource MigrationInputRecordId' AND Id = 1)
BEGIN
    SET IDENTITY_INSERT [hub].[Attribute] ON;

    ALTER TABLE [resources].[ResourceAttribute] DROP CONSTRAINT [FK_ResourceAttribute_Attribute];

    INSERT INTO [hub].[Attribute] ([Id],[AttributeTypeId],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (3, 1, 'Resource MigrationInputRecordId','If a resource was created during a migration, this attribute defines the Id of the MigrationInputRecord that it was originally created from.',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())

    UPDATE [resources].[ResourceAttribute] SET AttributeId = 3 where AttributeId = 1;

    -- Cannot update idenity column, just delete the row and let the post-deploy script add back the migration attribute with the correct id
    --UPDATE [hub].[Attribute] SET Id = 3 WHERE Name = 'Resource MigrationInputRecordId';
    
    DELETE FROM [hub].[Attribute] where Id = 1;

    INSERT INTO [hub].[Attribute] ([Id],[AttributeTypeId],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (1, 1,'Local Admin User Group','Local Admin User Group',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET());

	INSERT INTO [hub].[Attribute] ([Id],[AttributeTypeId],[Name],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
		VALUES (2, 2,'Restricted Access User Group','Local Admin User Group',0,4, SYSDATETIMEOFFSET(),4, SYSDATETIMEOFFSET());
    SET IDENTITY_INSERT [hub].[Attribute] OFF;
END
GO