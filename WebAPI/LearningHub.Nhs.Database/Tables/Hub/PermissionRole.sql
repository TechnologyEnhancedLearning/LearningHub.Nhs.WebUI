CREATE TABLE [hub].[PermissionRole](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PermissionId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_permissionRole] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [hub].[PermissionRole]  WITH CHECK ADD  CONSTRAINT [FK_permissionRole_permission] FOREIGN KEY([PermissionId])
REFERENCES [hub].[Permission] ([Id])
GO

ALTER TABLE [hub].[PermissionRole] CHECK CONSTRAINT [FK_permissionRole_permission]
GO

ALTER TABLE [hub].[PermissionRole]  WITH CHECK ADD  CONSTRAINT [FK_userPermissionRole_role] FOREIGN KEY([RoleId])
REFERENCES [hub].[Role] ([Id])
GO
