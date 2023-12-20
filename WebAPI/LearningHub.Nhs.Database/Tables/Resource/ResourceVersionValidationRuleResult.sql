CREATE TABLE [resources].[ResourceVersionValidationRuleResult](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionValidationResultId] [int] NOT NULL,
	[ResourceTypeValidationRuleId] [int] NOT NULL,
	[Success] [bit] NOT NULL,
	[Details] [nvarchar](1024) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ResourceVersionValidationRuleResult] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ResourceVersionValidationRuleResult] ADD  CONSTRAINT [FK_ResourceVersionValidationRuleResult_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionValidationRuleResult] ADD  CONSTRAINT [FK_ResourceVersionValidationRuleResult_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionValidationRuleResult] ADD  CONSTRAINT [FK_ResourceVersionValidationRuleResult_ResourceTypeValidationRule] FOREIGN KEY([ResourceTypeValidationRuleId])
REFERENCES [resources].[ResourceTypeValidationRule] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionValidationRuleResult] ADD  CONSTRAINT [FK_ResourceVersionValidationRuleResult_ResourceVersionValidationResult] FOREIGN KEY([ResourceVersionValidationResultId])
REFERENCES [resources].[ResourceVersionValidationResult] ([Id])
GO