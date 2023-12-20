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

IF NOT EXISTS (SELECT 1 FROM [messaging].[MessageType] WHERE Type = 'Email')
BEGIN
    INSERT INTO [messaging].[MessageType] (Id, Type, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'Email', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [messaging].[MessageType] WHERE Type = 'Notification')
BEGIN
    INSERT INTO [messaging].[MessageType] (Id, Type, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2, 'Notification', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END
