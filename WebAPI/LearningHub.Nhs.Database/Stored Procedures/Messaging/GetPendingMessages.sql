CREATE PROCEDURE [messaging].[GetPendingMessages]
	@MarkAsSending BIT
AS
BEGIN
	-- Create a temp table
	CREATE TABLE #Messages
	(
		Id INT IDENTITY(1,1),
		Subject NVARCHAR(512) NULL,
		Body NVARCHAR(MAX) NOT NULL,
		MessageId INT NOT NULL,
		MessageTypeId INT NOT NULL,
		UserId INT NULL, -- resolved user id, null for emails
		EmailAddress NVARCHAR(100) NULL, -- resolved email address, null for notificatons.
		UserGroupId INT NULL,
		MessageSendId INT NOT NULL,
		NotificationPriority INT NULL,
		NotificationType INT NULL,
		NotificationStartDate DATETIMEOFFSET(7) NULL,
		NotificationEndDate DATETIMEOFFSET(7) NULL,
	);

	-- are we sending back too much raw information? If a long email is to be sent to 10 user groups, each with 10 users, then that could duplicate the long body 100 times for no reason.
	-- Notifications
	INSERT INTO #Messages (Subject, Body, MessageId, MessageTypeId, UserId, EmailAddress, UserGroupId, MessageSendId, NotificationPriority, NotificationType, NotificationStartDate, NotificationEndDate)
	SELECT 
		m.Subject,
		m.Body,
		m.Id,
		ms.MessageTypeId,
		case 
			when msr.UserId IS NOT NULL AND msr.UserGroupId IS NULL THEN msr.UserId
			when msr.UserId IS NULL AND msr.UserGroupId IS NOT NULL AND uug.Id IS NOT NULL THEN uug.UserId
		END AS UserId,
		null as EmailAddress,
		ug.Id,
		ms.Id,
		mmd.NotificationPriority,
		mmd.NotificationType,
		mmd.NotificationStartDate,
		mmd.NotificationEndDate 
	from [messaging].[Message] m
	INNER JOIN [messaging].[MessageSend] ms ON ms.MessageId = m.Id AND ms.MessageTypeId = 2 -- Notifications
	INNER JOIN [messaging].[MessageSendRecipient] msr ON msr.MessageSendId = ms.Id
	INNER JOIN [messaging].[MessageMetaData] mmd on mmd.MessageId = m.Id
	LEFT OUTER JOIN [hub].[UserGroup] ug ON ug.Id = msr.UserGroupId AND ug.Deleted = 0 -- can be null
	LEFT OUTER JOIN [hub].[UserUserGroup] uug on ug.Id = uug.UserGroupId AND uug.Deleted = 0 -- can be null
	WHERE m.Deleted = 0
	AND ms.Deleted = 0 AND ms.Status = 0 /* Pending */
	AND msr.Deleted = 0;

	INSERT INTO #Messages (Subject, Body, MessageId, MessageTypeId, UserId, EmailAddress, UserGroupId, MessageSendId)
	SELECT 
		m.Subject,
		m.Body,
		m.Id,
		ms.MessageTypeId,
		null as UserId,
		msr.EmailAddress,
		ug.Id,
		ms.Id
	from [messaging].[Message] m
	INNER JOIN [messaging].[MessageSend] ms ON ms.MessageId = m.Id AND ms.MessageTypeId = 1 -- Emails
	INNER JOIN [messaging].[MessageSendRecipient] msr ON msr.MessageSendId = ms.Id
	-- UserGroupId
	LEFT OUTER JOIN [hub].[UserGroup] ug ON ug.Id = msr.UserGroupId AND ug.Deleted = 0 -- can be null
	LEFT OUTER JOIN [hub].[UserUserGroup] uug on uug.UserGroupId = ug.Id AND uug.Deleted = 0 -- can be null
	WHERE 
	msr.UserId IS NULL AND msr.UserGroupId IS NULL AND msr.EmailAddress IS NOT NULL
	AND m.Deleted = 0
	AND ms.Deleted = 0 AND ms.Status = 0 /* Pending */
	AND msr.Deleted = 0;

	INSERT INTO #Messages (Subject, Body, MessageId, MessageTypeId, UserId, EmailAddress, UserGroupId, MessageSendId)
	SELECT 
		m.Subject,
		m.Body,
		m.Id,
		ms.MessageTypeId,
		null as UserId,
		up.EmailAddress,
		ug.Id,
		ms.Id
	from [messaging].[Message] m
	INNER JOIN [messaging].[MessageSend] ms ON ms.MessageId = m.Id AND ms.MessageTypeId = 1 -- Emails
	INNER JOIN [messaging].[MessageSendRecipient] msr ON msr.MessageSendId = ms.Id
	-- UserGroupId
	LEFT OUTER JOIN [hub].[UserGroup] ug ON ug.Id = msr.UserGroupId AND ug.Deleted = 0 -- can be null
	LEFT OUTER JOIN [hub].[UserUserGroup] uug on uug.UserGroupId = ug.Id AND uug.Deleted = 0 -- can be null
	-- UserId
	INNER JOIN [hub].[UserProfile] up on up.Id = msr.UserId
	WHERE 
	msr.UserId IS NOT NULL AND msr.UserGroupId IS NULL AND msr.EmailAddress IS NULL
	AND m.Deleted = 0
	AND ms.Deleted = 0 AND ms.Status = 0 /* Pending */
	AND msr.Deleted = 0;

	INSERT INTO #Messages (Subject, Body, MessageId, MessageTypeId, UserId, EmailAddress, UserGroupId, MessageSendId)
	SELECT 
		m.Subject,
		m.Body,
		m.Id,
		ms.MessageTypeId,
		null as UserId,
		up.EmailAddress,
		ug.Id,
		ms.Id
	from [messaging].[Message] m
	INNER JOIN [messaging].[MessageSend] ms ON ms.MessageId = m.Id AND ms.MessageTypeId = 1 -- Emails
	INNER JOIN [messaging].[MessageSendRecipient] msr ON msr.MessageSendId = ms.Id
	-- UserGroupId
	LEFT OUTER JOIN [hub].[UserGroup] ug ON ug.Id = msr.UserGroupId AND ug.Deleted = 0 -- can be null
	LEFT OUTER JOIN [hub].[UserUserGroup] uug on uug.UserGroupId = ug.Id AND uug.Deleted = 0 -- can be null
	INNER JOIN [hub].[UserProfile] up on up.Id = uug.UserId AND up.Deleted = 0
	WHERE 
	msr.UserId IS NULL AND msr.UserGroupId IS NOT NULL AND msr.EmailAddress IS NULL
	AND m.Deleted = 0
	AND ms.Deleted = 0 AND ms.Status = 0 /* Pending */
	AND msr.Deleted = 0;


	IF @MarkAsSending = 1
	BEGIN
		UPDATE [messaging].[MessageSend] set Status = 1 /* Sending */ where Id in (SELECT DISTINCT MessageSendId from #Messages)
	END
	
	SELECT Id, Subject, Body, MessageId, MessageTypeId, UserId, EmailAddress, UserGroupId, MessageSendId, NotificationPriority, NotificationType, NotificationStartDate, NotificationEndDate
	FROM #Messages;

	DROP TABLE #Messages;
END