CREATE TABLE [hierarchy].[NodePathDisplay](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodePathId] [int] NOT NULL,
	[DisplayName] [nvarchar](128) NULL,
	[Icon] [nvarchar](128) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodePathDisplay] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hierarchy].[NodePathDisplay]  WITH CHECK ADD  CONSTRAINT [FK_NodePathDisplay_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO

ALTER TABLE [hierarchy].[NodePathDisplay] CHECK CONSTRAINT [FK_NodePathDisplay_NodePath]
GO
