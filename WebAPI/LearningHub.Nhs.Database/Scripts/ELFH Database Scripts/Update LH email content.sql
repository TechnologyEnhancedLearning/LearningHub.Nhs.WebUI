
IF NOT EXISTS(SELECT 1 FROM emailTemplateTBL Where emailTemplateTypeId = 3 and tenantId = 10)
BEGIN
    INSERT INTO emailTemplateTBL (emailTemplateTypeID, programmeComponentId, tenantId, title, subject, body, deleted, amendUserId)
    Values (3, 0, 10, 'Learning Hub New User', 'New e-Learning account', '', 0, 0)
END

IF NOT EXISTS(SELECT 1 FROM emailTemplateTBL Where emailTemplateTypeId = 12 and tenantId = 10)
BEGIN
    INSERT INTO emailTemplateTBL (emailTemplateTypeID, programmeComponentId, tenantId, title, subject, body, deleted, amendUserId)
    Values (12, 0, 10, 'User Password Validate Email', 'New Learning Hub account', '', 0, 0)
END

IF NOT EXISTS(SELECT 1 FROM emailTemplateTBL Where emailTemplateTypeId = 21 and tenantId = 10)
BEGIN
    INSERT INTO emailTemplateTBL (emailTemplateTypeID, programmeComponentId, tenantId, title, subject, body, deleted, amendUserId)
    Values (21, 0, 10, 'Forgot UserName Email', 'New Learning Hub account', '', 0, 0)
END


-- New Registration
UPDATE emailTemplateTBL
SET body = '<p style ="color: #007bff">This is an automated email, please do not reply to this address</p>
Dear [FullName],<br/><br/>  Thank you for creating an account on the Learning Hub.<br/><br/>  
Your username is <b>[UserName]</b>, You now need to <b><a href="[PasswordValidateUrl]">add a password</a></b>.<br/><br/>  
In the unlikely event that selecting the above link does not work, please copy and paste this directly into the address bar of your browser: [PasswordValidateUrl]<br/><br/>  
Note that this link has a time limit of [TimeLimit]. If you select the link after this time you will receive instructions on how to generate a new one.<br/><br/>  
To log in at any other time go to <a href="[TenantUrl]">[TenantUrl]</a>, enter your username and password and select the Sign in button.<br/><br/>  
When you first log in you will be guided through a series of steps to complete your registration. Providing correct information means that we can offer you a better learning experience.<br/><br/>  
Please <b>do not reply</b> to this email.<br/><br/>  
For support: <ul>  <li>Visit the <a href="https://support.learningub.nhs.uk/">Support pages</a> for login guidance, learning resources, and contribution options. </li>  <li>If you have any issues or questions, contact us via the <a href="[SupportFormUrl]">Support Form</a>.</li>  <li>Use Live Chat (look for the icon in the bottom right of your browser) to connect directly with the Support Team.</li>  </ul>  Thank you for registering, we hope you enjoy your learning experience.<br/><br/>  
The Learning Hub Support Team<br/>  
NHS England<br/><br/>  
<small>This e-mail and any files transmitted with it are confidential. If you are not the intended recipient, any reading, printing, storage, disclosure, copying or any other action taken in respect of this e-mail is prohibited and may be unlawful. If you are not the intended recipient, please notify the sender immediately by using the reply function and then permanently delete what you have received.<br/></small><br/>
'
Where emailTemplateTypeId = 3 
  and tenantId = 10


-- User Password Validate Email
UPDATE emailTemplateTBL
SET body = '<p style ="color: #007bff">This is an automated email, please do not reply to this address</p>
Dear [FullName],<br/><br/>  
We received a request to reset your password for the Learning Hub.<br/><br/>  
If this was not you, contact us using the following <a href="[SupportFormUrl]">Support Form</a>.<br/><br/>  Your username is [UserName]. You now need to <b><a href="[PasswordValidateUrl]">add a password</a></b>.<br/><br/>  
In the unlikely event that selecting the above link does not work, please copy and paste this directly into the address bar of your browser: [PasswordValidateUrl]<br/><br/>  
Note that this link has a time limit of [TimeLimit]. If you select the link after this time you will receive instructions on how to generate a new one.<br/><br/>  
Once you have reset your password you will be given a link to log in with your new password.<br/><br/>  
Please <b>do not reply</b> to this email.<br/><br/>    
For support: <ul>  <li>Visit the <a href="https://support.learningub.nhs.uk/">Support pages</a> for login guidance, learning resources, and contribution options. </li>  <li>If you have any issues or questions, contact us via the <a href="[SupportFormUrl]">Support Form</a>.</li>  <li>Use Live Chat (look for the icon in the bottom right of your browser) to connect directly with the Support Team.</li>  </ul> 
We hope you enjoy your learning experience.<br/><br/>  The Learning Hub Support Team<br/>  
NHS England<br/><br/>  
<small>This e-mail and any files transmitted with it are confidential. If you are not the intended recipient, any reading, printing, storage, disclosure, copying or any other action taken in respect of this e-mail is prohibited and may be unlawful. If you are not the intended recipient, please notify the sender immediately by using the reply function and then permanently delete what you have received.<br/></small><br/>
'
Where emailTemplateTypeId = 12 
  and tenantId = 10


-- Forgot UserName Email
UPDATE emailTemplateTBL
SET body = '<p style ="color: #007bff">This is an automated email, please do not reply to this address</p>
Dear [FullName],<br/><br/>  
We received a request to retrieve your username for the Learning Hub.<br/><br/>  
If this was not you, contact us using the following <a href="[SupportFormUrl]">Support Form</a>.<br/><br/>  
The username registered to this email address is: [UserName]<br/><br/>  
If you know your password, you can log in straight away or do you need a <b><a href="[TenantUrl]/forgotten-password">password reminder</a></b> password reminder?<br/><br/>  
Please <b>do not reply</b> to this email.<br/><br/>   
For support: <ul>  <li>Visit the <a href="https://support.learningub.nhs.uk/">Support pages</a> for login guidance, learning resources, and contribution options. </li>  <li>If you have any issues or questions, contact us via the <a href="[SupportFormUrl]">Support Form</a>.</li>  <li>Use Live Chat (look for the icon in the bottom right of your browser) to connect directly with the Support Team.</li>  </ul>  
We hope you enjoy your learning experience.<br/><br/>  
The Learning Hub Support Team<br/> 
NHS England<br/><br/>  
<small>This e-mail and any files transmitted with it are confidential. If you are not the intended recipient, any reading, printing, storage, disclosure, copying or any other action taken in respect of this e-mail is prohibited and may be unlawful. If you are not the intended recipient, please notify the sender immediately by using the reply function and then permanently delete what you have received.<br/></small><br/>  
'
Where emailTemplateTypeId = 21 
  and tenantId = 10
