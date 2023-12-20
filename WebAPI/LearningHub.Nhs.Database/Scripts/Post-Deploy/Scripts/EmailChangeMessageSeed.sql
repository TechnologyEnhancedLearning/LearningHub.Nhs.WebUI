IF NOT EXISTS (SELECT 1 FROM [messaging].[EmailTemplate] WHERE Title = 'EmailChangeConfirmationEmail')
BEGIN
    INSERT INTO [messaging].[EmailTemplate] (Id, LayoutId, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2004, 1, 'EmailChangeConfirmationEmail', 'Thank you for upgrading your Learning Hub account', 
	'<p>Dear User,</p>
<p>Thank you for updating your email address on the Learning Hub.</p>

<p>Please <a href="[ValidationTokenUrl]"> validate your new email address.</a></p>

<p>In the unlikely event that selecting the above link does not work, please copy and paste this directly into the address bar of your browser: <a href="[ValidationTokenUrl]">[ValidationTokenUrl]</a></p>

<p>Your Account will be updated with the new email address but will appear disabled until we have processed this email confirmation.</p>

<p>Note that this link has a time limit and will expire on date [ExpiredDate] and time [ExpiredTime]. If the link has expired, you can go through My Account and request that a new validation email is resent or you can cancel the email address change.</p>

<p>Please do not reply to this email.</p>

<ol>
<li>For more information on how to log in, access learning resources and contribute resources, visit the <a href="[SupportPages]">Learning Hub Support pages</a>.</li>
<li>If you have any problems or further questions, contact us using the following <a href="[SupportForm]">Support Form</a>.</li>
</ol>

<p>Thanks again for upgrading to a Full user account, and we hope you enjoy your learning experience.</p>
<p>The Learning Hub Support Team, NHS England.</p>
', '[UserFirstName][UserName][EmailAddress][ValidationTokenUrl][ExpiredDate][ExpiredTime][SupportForm][SupportPages]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [messaging].[EmailTemplate] WHERE Title = 'EmailVerified')
BEGIN
    INSERT INTO [messaging].[EmailTemplate] (Id, LayoutId, Title, Subject, Body, AvailableTags, Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    VALUES (2005, 1, 'EmailVerified', 'Your new email has been verified', 
	'<p>Dear User,</p>

<p>Thank you for verifying your email address on the Learning Hub.</p>

<p>Please now log in to the Learning Hub using your existing username and password.</p>

<p>If you did not request this change, please contact the <a href="[SupportForm]">support team.</a></p>

<p>Thanks again for upgrading to a Full user account, and we hope you enjoy your learning experience.</p>
<p>The Learning Hub Support Team, NHS England.</p>', '[SupportForm]', 0, 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET());
END

IF NOT EXISTS (SELECT 1 FROM [hub].[EmailChangeValidationTokenStatus] WHERE Id > 0)
BEGIN
	INSERT  [hub].[EmailChangeValidationTokenStatus] ([Id], [Name], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted]) VALUES (1, N'Issued', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
	INSERT  [hub].[EmailChangeValidationTokenStatus] ([Id], [Name], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted]) VALUES (2, N'Abandoned', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
	INSERT  [hub].[EmailChangeValidationTokenStatus] ([Id], [Name], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted]) VALUES (3, N'Cancelled', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
	INSERT  [hub].[EmailChangeValidationTokenStatus] ([Id], [Name], [CreateUserID], [CreateDate], [AmendUserID], [AmendDate], [Deleted]) VALUES (4, N'Completed', 4, SYSDATETIMEOFFSET(), 4, SYSDATETIMEOFFSET(), 0)
END