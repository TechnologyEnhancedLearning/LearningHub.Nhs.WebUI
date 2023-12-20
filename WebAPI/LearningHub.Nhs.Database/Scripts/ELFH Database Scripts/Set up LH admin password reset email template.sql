
IF NOT EXISTS(SELECT 1 FROM [dbo].[emailTemplateTBL] WHERE emailTemplateTypeId = 13 AND tenantId = 10)
BEGIN

INSERT INTO [dbo].[emailTemplateTBL] (
	emailTemplateTypeId,
	programmeComponentId,
	tenantId,
	title,
	subject,
	body,
	deleted,
	amendUserID,
	amendDate)
VALUES (
13,
11,
10,
'Admin Password Validate Email',
'Learning Hub username reminder and password reset',
'',
0,
4,
SYSDATETIMEOFFSET())

END

UPDATE emailTemplateTBL
SET body = '<p style ="color: #007bff">This is an automated email, please do not reply to this address</p>
Dear [FullName],<br /><br /> 
We received a request to retrieve your username or reset your password for the Learning Hub. If this was not you, please inform our <a href="[SupportFormUrl]">support team</a>.<br /><br /> 
The username linked to this email address is: [UserName]<br /><br /> 
If you know your password, you can <a href="https://learninghub.nhs.uk/">sign in</a> straight away. If not, you need to <a href="[PasswordValidateUrl]">create a new password</a>.<br /><br /> 
If the link to create a new password does not work, please copy and paste this directly into the address bar of your browser:<br />[PasswordValidateUrl]<br /><br />  
This link will expire in [TimeLimit]. If you select the link after this time you will receive instructions on how to get  a new one.<br /><br /> 
<br /><br /> For more information on how to access and contribute learning resources, visit the  Learning Hub <a href="https://support.learninghub.nhs.uk">support site</a>.<br /><br /> 
If you have any problems or  further questions, contact our <a href="[SupportFormUrl]">support team</a>.<br /><br /> We hope you enjoy your learning  experience.<br /><br /> 
Learning Hub support team<br /><br /> <a href="https://learninghub.nhs.uk">https://learninghub.nhs.uk</a> <br /><br />
<b>NHSE e-Learning for Healthcare</b><br /> 
(NHS England in partnership with the NHS and professional bodies)<br /><br />  
<small>This email  and any files transmitted with it are confidential. If you are not the intended recipient, any reading, printing, storage, disclosure, copying or any other action taken in respect of this e-mail is prohibited and may be unlawful. If you are not the intended recipient, please notify the sender immediately by using the reply function and then permanently delete what you have received.</small><br />
'
Where emailTemplateTypeId = 13 
  and tenantId = 10
