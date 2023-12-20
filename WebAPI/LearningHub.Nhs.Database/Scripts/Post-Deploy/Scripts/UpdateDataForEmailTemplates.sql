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


UPDATE [messaging].[EmailTemplate]
SET Body = '<p>Dear [AdminFirstName],</p>
    <p>A user has requested access to the [CatalogueName] catalogue in the Learning Hub. Please review this request and then approve or deny access in the Learning Hub by selecting the link below.</p>
    <hr></hr>
    <br/>
    <span><b>Requested by</b></span><br/>
    <span>[UserFullName]</span><br/>
    <span>[UserEmailAddress]</span><br/>
    <br/>
    <span><b style="margin-top: 10px;">Message</b></span><br/>
    <span>[UserMessage]</span><br/>
    <br/>
    <a href="[ManageAccessUrl]" style="font-size: 16px;">Review request</a>' 
where Title = 'CatalogueAccessRequest';

UPDATE [messaging].[EmailTemplate]
SET Body = '<p>Dear [UserFirstName],</p>
    <p>Your request to access the <a href="[CatalogueUrl]">[CatalogueName]</a> catalogue has been approved.</p>
    <hr></hr>'
where Title = 'CatalogueAccessRequestSuccess';

UPDATE [messaging].[EmailTemplate]
SET Body = '<p>Dear [UserFirstName],</p>
    <p>The catalogue administrator did not approve your request to access the <a href="[CatalogueUrl]">[CatalogueName]</a> catalogue. The following reason was given:</p>
    <p>[RejectionReason]</p>
    <hr></hr>'
where Title = 'CatalogueAccessRequestFailure';

UPDATE [messaging].[EmailTemplate]
SET Body = '<p>[Greeting],</p>
    <p>The catalogue administrator of the <a href="[CatalogueUrl]">[CatalogueName]</a> catalogue, has sent you a link to request access to it.</p><p>You can sign into the Learning Hub either using an e-Learning for Healthcare username and password or NHS OpenAthens user account details or by creating an account on the Learning Hub and using those details.</p>'
where Title = 'CatalogueAccessInvitation';

UPDATE [messaging].[EmailTemplateLayout]
SET Body = '<div style="font-family: Arial; line-height: 19px; font-size: 13px">
    [Content]
</div>
<br/>
<p style="font-family: Arial; font-size: 13px">Please <b>do not reply</b> to this email.</p>
<p><b style="font-family: Arial; font-size: 13px">Learning Hub support team</b></p>
<p style="font-family: Arial; font-size: 13px">NHS England</p>
<p><a style="font-family: Arial; font-size: 13px" href="https://learninghub.nhs.uk">https://learninghub.nhs.uk</a></p><br/>
<p style="font-family: Arial; font-size: 13px; line-height: 19px">This email and any files transmitted with it are confidential. If you are not the intended recipient, any reading, printing, storage, disclosure, copying or any other action taken in respect of this e-mail is prohibited and may be unlawful. If you are not the intended recipient, please notify the sender immediately by using the reply function and then permanently delete what you have received.</p>' 
WHERE Name = 'LearningHub';