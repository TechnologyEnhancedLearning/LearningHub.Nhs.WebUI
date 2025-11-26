CREATE TABLE [elfh].[userPasswordValidationTokenTBL](
	[userPasswordValidationTokenId] [int] IDENTITY(1,1) NOT NULL,
	[hashedToken] [nvarchar](128) NOT NULL,
	[salt] [nvarchar](128) NOT NULL,
	[lookup] [nvarchar](128) NOT NULL,
	[expiry] [datetimeoffset](7) NOT NULL,
	[tenantId] [int] NOT NULL,
	[userId] [int] NOT NULL,
	[createdUserId] [int] NOT NULL,
	[createdDate] [datetimeoffset](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[userPasswordValidationTokenId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userPasswordValidationTokenTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [createdDate]
GO

ALTER TABLE [elfh].[userPasswordValidationTokenTBL]  WITH CHECK ADD  CONSTRAINT [FK_userPasswordValidationTokenTBL_tenantTBL] FOREIGN KEY([tenantId])
REFERENCES [elfh].[tenantTBL] ([tenantId])
GO

ALTER TABLE [elfh].[userPasswordValidationTokenTBL] CHECK CONSTRAINT [FK_userPasswordValidationTokenTBL_tenantTBL]
GO

ALTER TABLE [elfh].[userPasswordValidationTokenTBL]  WITH CHECK ADD  CONSTRAINT [FK_userPasswordValidationTokenTBL_userTBL] FOREIGN KEY([userId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [elfh].[userPasswordValidationTokenTBL] CHECK CONSTRAINT [FK_userPasswordValidationTokenTBL_userTBL]
GO