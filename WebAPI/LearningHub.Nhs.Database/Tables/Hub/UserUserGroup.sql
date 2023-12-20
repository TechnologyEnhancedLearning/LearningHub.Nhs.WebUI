CREATE TABLE [hub].[UserUserGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[UserGroupId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userUserGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[UserUserGroup]  WITH CHECK ADD  CONSTRAINT [FK_userUserGroup_user] FOREIGN KEY([UserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hub].[UserUserGroup]  WITH CHECK ADD  CONSTRAINT [FK_userUserGroup_userGroup] FOREIGN KEY([UserGroupId])
REFERENCES [hub].[UserGroup] ([Id])
GO

CREATE INDEX IX_UserUserGroup_UserId ON hub.UserUserGroup(UserId) WITH (FILLFACTOR = 95);
GO

CREATE INDEX IX_UserUserGroup_UserGroupId ON hub.UserUserGroup(UserGroupId) WITH (FILLFACTOR = 95);
GO