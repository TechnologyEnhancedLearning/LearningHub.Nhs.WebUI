CREATE TABLE [elfh].[userGroupTypeInputValidationTBL](
	[userGroupTypeInputValidationId] [int] IDENTITY(1,1) NOT NULL,
	[userGroupId] [int] NOT NULL,
	[userGroupTypePrefix] [nvarchar](10) NOT NULL,
	[userGroupTypeId] [int] NOT NULL,
	[validationTextValue] [nvarchar](1000) NOT NULL,
	[validationMethod] [int] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
	[createdUserId] [int] NOT NULL,
	[createdDate] [datetimeoffset](7) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[userGroupTypeInputValidationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_userGroupTypeInputValidationTBL_userGroupId_validationTextValue] UNIQUE NONCLUSTERED 
(
	[userGroupId] ASC,
	[validationTextValue] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [elfh].[userGroupTypeInputValidationTBL] ADD  DEFAULT (N'USR TYP:') FOR [userGroupTypePrefix]
GO

ALTER TABLE [elfh].[userGroupTypeInputValidationTBL] ADD  DEFAULT ((0)) FOR [deleted]
GO

ALTER TABLE [elfh].[userGroupTypeInputValidationTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [amendDate]
GO

ALTER TABLE [elfh].[userGroupTypeInputValidationTBL] ADD  DEFAULT (sysdatetimeoffset()) FOR [createdDate]
GO

ALTER TABLE [elfh].[userGroupTypeInputValidationTBL]  WITH CHECK ADD  CONSTRAINT [FK_userGroupTypeInputValidationTBL_amendUserTBL] FOREIGN KEY([amendUserId])
REFERENCES [hub].[User] ([Id])
GO