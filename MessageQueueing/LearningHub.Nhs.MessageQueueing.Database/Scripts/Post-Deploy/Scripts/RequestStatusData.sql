IF NOT EXISTS(SELECT Id FROM [dbo].[RequestStatus] WHERE Id = 1)
BEGIN
INSERT [dbo].[RequestStatus] ([Id], [RequestStatus]) VALUES (1, N'Pending')
END

IF NOT EXISTS(SELECT Id FROM [dbo].[RequestStatus] WHERE Id = 2)
BEGIN
INSERT [dbo].[RequestStatus] ([Id], [RequestStatus]) VALUES (2, N'Sent')
END

IF NOT EXISTS(SELECT Id FROM [dbo].[RequestStatus] WHERE Id = 3)
BEGIN
INSERT [dbo].[RequestStatus] ([Id], [RequestStatus]) VALUES (3, N'Failed')
END