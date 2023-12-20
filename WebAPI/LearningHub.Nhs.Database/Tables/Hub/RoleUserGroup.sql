CREATE TABLE [hub].[RoleUserGroup](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RoleId] [int] NOT NULL,
	[UserGroupId] [int] NOT NULL,
	[ScopeId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userRoleUserGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[RoleUserGroup] ADD  CONSTRAINT [DF_roleUserGroup_amendDate]  DEFAULT (sysdatetimeoffset()) FOR [AmendDate]
GO

ALTER TABLE [hub].[RoleUserGroup]  WITH CHECK ADD  CONSTRAINT [FK_userRoleUserGroup_scope] FOREIGN KEY([ScopeId])
REFERENCES [hub].[Scope] ([Id])
GO

ALTER TABLE [hub].[RoleUserGroup] CHECK CONSTRAINT [FK_userRoleUserGroup_scope]
GO

ALTER TABLE [hub].[RoleUserGroup]  WITH CHECK ADD  CONSTRAINT [FK_userRoleUserGroup_userGroup] FOREIGN KEY([UserGroupId])
REFERENCES [hub].[UserGroup] ([Id])
GO

ALTER TABLE [hub].[RoleUserGroup] CHECK CONSTRAINT [FK_userRoleUserGroup_userGroup]
GO

ALTER TABLE [hub].[RoleUserGroup]  WITH CHECK ADD  CONSTRAINT [FK_userRoleUserGroup_userRole] FOREIGN KEY([RoleId])
REFERENCES [hub].[Role] ([Id])
GO

ALTER TABLE [hub].[RoleUserGroup] CHECK CONSTRAINT [FK_userRoleUserGroup_userRole]
GO
