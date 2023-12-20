DECLARE @now datetimeoffset(7) = SYSDATETIMEOFFSET()


IF NOT EXISTS(SELECT Id FROM [hub].[NotificationType] WHERE [Name] = 'SystemUpdate')
BEGIN
	SET IDENTITY_INSERT [hub].[NotificationType] ON

	INSERT INTO [hub].[NotificationType] 
		([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
	VALUES 
		(1, 'SystemUpdate', 'System update', 0, 4, @now, 4, @now),
		(2, 'SystemRelease', 'System release', 0, 4, @now, 4, @now),
		(3, 'ActionRequired', 'Action required', 0, 4, @now, 4, @now),
		(4, 'ResourcePublished', 'Published', 0, 4, @now, 4, @now),
		(5, 'ResourceRated', 'New rating', 0, 4, @now, 4, @now),
		(6, 'UserPermission', 'Change of permissions', 0, 4, @now, 4, @now),
		(7, 'PublishFailed', 'Action required', 0, 4, @now, 4, @now)

	SET IDENTITY_INSERT [hub].[NotificationType] OFF
END



IF NOT EXISTS(SELECT Id FROM [hub].[NotificationPriority] WHERE [Name] = 'General')
BEGIN
	SET IDENTITY_INSERT [hub].[NotificationPriority] ON

	INSERT INTO [hub].[NotificationPriority] 
		([Id], [Name], [Description], [Deleted], [CreateUserId], [CreateDate], [AmendUserId], [AmendDate])
	VALUES 
		(1, 'General', 'Other', 0, 4, @now, 4, @now),
		(2, 'Priority', 'High priority', 0, 4, @now, 4, @now)

	SET IDENTITY_INSERT [hub].[NotificationPriority] OFF
END



UPDATE [hub].[Notification] SET [NotificationTypeId] = 1 WHERE [NotificationTypeId] IS NULL

UPDATE [hub].[Notification] SET [NotificationPriorityId] = 1 WHERE [NotificationPriorityId] IS NULL

UPDATE [hub].[UserNotification] SET [ReadOnDate] = [AmendDate]