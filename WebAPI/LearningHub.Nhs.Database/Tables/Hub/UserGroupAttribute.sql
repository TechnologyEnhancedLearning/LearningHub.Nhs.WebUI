CREATE TABLE [hub].[UserGroupAttribute](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserGroupId] [int] NOT NULL,
	[AttributeId] [int] NOT NULL,
	[ScopeId] [int] NULL,
	[IntValue] [int] NULL,
	[TextValue] [nvarchar](255) NULL,
	[BooleanValue] [bit] NULL,
	[DateValue] [datetimeoffset](7) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userGroupAttribute] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[UserGroupAttribute]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupAttribute_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hub].[UserGroupAttribute] CHECK CONSTRAINT [FK_UserGroupAttribute_AmendUser]
GO

ALTER TABLE [hub].[UserGroupAttribute]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupAttribute_Attribute] FOREIGN KEY([AttributeId])
REFERENCES [hub].[Attribute] ([Id])
GO

ALTER TABLE [hub].[UserGroupAttribute] CHECK CONSTRAINT [FK_UserGroupAttribute_Attribute]
GO

ALTER TABLE [hub].[UserGroupAttribute]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupAttribute_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hub].[UserGroupAttribute] CHECK CONSTRAINT [FK_UserGroupAttribute_CreateUser]
GO

ALTER TABLE [hub].[UserGroupAttribute]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupAttribute_Scope] FOREIGN KEY([ScopeId])
REFERENCES [hub].[Scope] ([Id])
GO

ALTER TABLE [hub].[UserGroupAttribute] CHECK CONSTRAINT [FK_UserGroupAttribute_Scope]
GO

ALTER TABLE [hub].[UserGroupAttribute]  WITH CHECK ADD  CONSTRAINT [FK_UserGroupAttribute_UserGroup] FOREIGN KEY([UserGroupId])
REFERENCES [hub].[UserGroup] ([Id])
GO

ALTER TABLE [hub].[UserGroupAttribute] CHECK CONSTRAINT [FK_UserGroupAttribute_UserGroup]
GO
