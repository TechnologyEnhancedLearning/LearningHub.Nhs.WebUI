CREATE TABLE [hierarchy].[HierarchyEdit](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RootNodePathId] [int] NULL,
	[HierarchyEditStatusId] [int] NULL,
	[PublicationId] [int] NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_HierarchyEdit] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [hierarchy].[HierarchyEdit]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEdit_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEdit] CHECK CONSTRAINT [FK_HierarchyEdit_AmendUser]
GO

ALTER TABLE [hierarchy].[HierarchyEdit]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEdit_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEdit] CHECK CONSTRAINT [FK_HierarchyEdit_CreateUser]
GO

ALTER TABLE [hierarchy].[HierarchyEdit]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEdit_HierarchyEditStatus] FOREIGN KEY([HierarchyEditStatusId])
REFERENCES [hierarchy].[HierarchyEditStatus] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEdit] CHECK CONSTRAINT [FK_HierarchyEdit_HierarchyEditStatus]
GO


ALTER TABLE [hierarchy].[HierarchyEdit]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEdit_NodePath] FOREIGN KEY([RootNodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEdit] CHECK CONSTRAINT [FK_HierarchyEdit_NodePath]
GO


ALTER TABLE [hierarchy].[HierarchyEdit] WITH CHECK ADD  CONSTRAINT [FK_HierarchyEdit_Publication] FOREIGN KEY([PublicationId])
REFERENCES [hierarchy].[Publication] ([Id])