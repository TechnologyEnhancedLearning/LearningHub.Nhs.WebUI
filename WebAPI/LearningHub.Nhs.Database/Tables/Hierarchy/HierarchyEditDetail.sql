CREATE TABLE [hierarchy].[HierarchyEditDetail](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HierarchyEditId] [int] NOT NULL,
	[HierarchyEditDetailTypeId] [int] NOT NULL,
	[HierarchyEditDetailOperationId] [int] NULL,
	[NodeId] [int] NULL,
	[NodeVersionId] [int] NULL,
	[ParentNodeId] [int] NULL,
	[NodeLinkId] [int] NULL,
	[ResourceId] [int] NULL,
	[ResourceVersionId] [int] NULL,
	[NodeResourceId] [int] NULL,
	[DisplayOrder] [int] NULL,
	[InitialNodePath] [nvarchar](256) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_HierarchyEditDetail] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEditDetail_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail] CHECK CONSTRAINT [FK_HierarchyEditDetail_AmendUser]
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEditDetail_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail] CHECK CONSTRAINT [FK_HierarchyEditDetail_CreateUser]
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEditDetail_HierarchyEdit] FOREIGN KEY([HierarchyEditId])
REFERENCES [hierarchy].[HierarchyEdit] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail] CHECK CONSTRAINT [FK_HierarchyEditDetail_HierarchyEdit]
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEditDetail_HierarchyEditDetailOperation] FOREIGN KEY([HierarchyEditDetailOperationId])
REFERENCES [hierarchy].[HierarchyEditDetailOperation] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail] CHECK CONSTRAINT [FK_HierarchyEditDetail_HierarchyEditDetailOperation]
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEditDetail_HierarchyEditDetailType] FOREIGN KEY([HierarchyEditDetailTypeId])
REFERENCES [hierarchy].[HierarchyEditDetailType] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEditDetail] CHECK CONSTRAINT [FK_HierarchyEditDetail_HierarchyEditDetailType]
GO