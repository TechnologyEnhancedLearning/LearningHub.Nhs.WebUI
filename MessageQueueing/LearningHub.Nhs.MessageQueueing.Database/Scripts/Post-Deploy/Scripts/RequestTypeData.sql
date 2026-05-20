IF NOT EXISTS(SELECT Id FROM [dbo].[RequestType] WHERE Id = 1)
BEGIN
INSERT [dbo].[RequestType] ([Id], [RequestType]) VALUES (1, N'Email')
END

IF NOT EXISTS(SELECT Id FROM [dbo].[RequestType] WHERE Id = 2)
BEGIN
INSERT [dbo].[RequestType] ([Id], [RequestType]) VALUES (2, N'SMS')
END

IF NOT EXISTS(SELECT Id FROM [dbo].[RequestType] WHERE Id = 3)
BEGIN
INSERT [dbo].[RequestType] ([Id], [RequestType]) VALUES (3, N'SingleEmail')
END