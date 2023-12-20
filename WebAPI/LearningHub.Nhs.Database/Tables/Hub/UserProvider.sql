CREATE TABLE [hub].[UserProvider](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ProviderId] [int] NOT NULL,
	[RemovalDate] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userProvider] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[UserProvider]  WITH CHECK ADD  CONSTRAINT [FK_userProvider_user] FOREIGN KEY([UserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hub].[UserProvider]  WITH CHECK ADD  CONSTRAINT [FK_userProvider_provider] FOREIGN KEY([ProviderId])
REFERENCES [hub].[Provider] ([Id])
GO

CREATE INDEX IX_UserProvider_UserId ON hub.UserProvider(UserId) WITH (FILLFACTOR = 95);
GO

CREATE INDEX IX_UserProvider_ProviderId ON hub.UserProvider(ProviderId) WITH (FILLFACTOR = 95);
GO