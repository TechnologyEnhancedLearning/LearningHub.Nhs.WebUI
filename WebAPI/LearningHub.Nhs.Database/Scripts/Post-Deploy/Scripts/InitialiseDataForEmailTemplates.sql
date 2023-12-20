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
IF NOT EXISTS (SELECT 1 FROM [messaging].[EmailTemplateLayout] WHERE Name = 'LearningHub')
BEGIN
    INSERT INTO [messaging].[EmailTemplateLayout] (Id, Name, Body, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (1, 'LearningHub', '<div style="font-family: Arial">
    [Content]
</div>
<br/>
<p style="font-family: Arial">Please <b>do not reply</b> to this email.</p>
<p><b style="font-family: Arial">Learning Hub support team</b></p>
<p style="font-family: Arial">Health Education England</p>
<p><a style="font-family: Arial" href="https://learninghub.nhs.uk">https://learninghub.nhs.uk</a></p><br/>
<p style="font-family: Arial">This email and any files transmitted with it are confidential. If you are not the intended recipient, any reading, printing, storage, disclosure, copying or any other action taken in respect of this e-mail is prohibited and may be unlawful. If you are not the intended recipient, please notify the sender immediately by using the reply function and then permanently delete what you have received.</p>', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [messaging].[EmailTemplate] WHERE Title = 'CatalogueAccessRequest')
BEGIN
    INSERT INTO [messaging].[EmailTemplate] (Id, LayoutId, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2000, 1, 'CatalogueAccessRequest', 'Learning Hub - catalogue access request', '<p>Dear [AdminFirstName],</p>
    <br/>
    <p>A user has requested access to the [CatalogueName] catalogue in the Learning Hub. Please review this request and then approve or deny access in the Learning Hub by selecting the link below.</p>
    <div style="width: 100%; height: 0; border-top: 1px solid #AEB7BD; margin-top: 10px; margin-bottom: 10px;"></div>
    <p><b>Requested by</b></p>
    <p>[UserFullName]</p>
    <p>[UserEmailAddress]</p>
    </br>
    <p><b style="margin-top: 10px;">Message</b></p>
    <p>[UserMessage]</p>
    <br/>
    <a href="[ManageAccessUrl]" style="font-size: 24px;">Review request</a>', '[AdminFirstName][CatalogueName][UserFullName][UserEmailAddress][UserMessage][ManageAccessUrl]',0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [messaging].[EmailTemplate] WHERE Title = 'CatalogueAccessRequestSuccess')
BEGIN
    INSERT INTO [messaging].[EmailTemplate] (Id, LayoutId, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2001, 1, 'CatalogueAccessRequestSuccess', 'Learning Hub - catalogue access request approved', '<p>Dear [UserFirstName],</p><br/>
    <p>Your request to access the <a href="[CatalogueUrl]">[CatalogueName]</a> catalogue has been approved.</p>
    <div style="width: 100%; height: 0; border-top: 1px solid #AEB7BD; margin-top: 26px;"></div>', '[UserFirstName][CatalogueUrl][CatalogueName]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [messaging].[EmailTemplate] WHERE Title = 'CatalogueAccessRequestFailure')
BEGIN
    INSERT INTO [messaging].[EmailTemplate] (Id, LayoutId, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2002, 1, 'CatalogueAccessRequestFailure', 'Learning Hub - catalogue access request denied', '<p>Dear [UserFirstName],</p><br/>
    <p>The catalogue administrator did not approve your request to access the <a href="[CatalogueUrl]">[CatalogueName]</a> catalogue. The following reason was given:</p>
    <p>[RejectionReason]</p>
    <div style="width: 100%; height: 0; border-top: 1px solid #AEB7BD; margin-top: 26px;"></div>', '[UserFirstName][CatalogueName][CatalogueUrl][RejectionReason]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [messaging].[EmailTemplate] WHERE Title = 'CatalogueAccessInvitation')
BEGIN
    INSERT INTO [messaging].[EmailTemplate] (Id, LayoutId, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2003, 1, 'CatalogueAccessInvitation', 'Learning Hub - request access to a catalogue', '<p>[Greeting],</p><br/>
    <p>The catalogue administrator of the <a href="[CatalogueUrl]">[CatalogueName]</a> catalogue, has sent you a link to request access to it.</p><br/><p>You can sign into the Learning Hub either using an e-Learning for Healthcare username and password or NHS OpenAthens user account details or by creating an account on the Learning Hub and using those details.</p>', '[Greeting][AdminFullName][CatalogueName][CatalogueUrl][CreateAccountUrl]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END