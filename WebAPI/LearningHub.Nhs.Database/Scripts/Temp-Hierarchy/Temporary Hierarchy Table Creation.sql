/* KD - 17.2.20 - Temporary script to create the tables in the hierarchy schema
- workaround for issue with "retention unit" in VS script generation for SQL Server 2016*/
ALTER TABLE [hierarchy].[NodeVersion] DROP CONSTRAINT [FK_NodeVersion_Node]
GO
ALTER TABLE [hierarchy].[NodeResource] DROP CONSTRAINT [FK_NodeResource_Resource]
GO
ALTER TABLE [hierarchy].[NodeResource] DROP CONSTRAINT [FK_NodeResource_Node]
GO
ALTER TABLE [hierarchy].[NodePathResource] DROP CONSTRAINT [FK_NodePathResource_Resource]
GO
ALTER TABLE [hierarchy].[NodePathResource] DROP CONSTRAINT [FK_NodePathResource_NodePath]
GO
ALTER TABLE [hierarchy].[NodePathNode] DROP CONSTRAINT [FK_NodePathNode_NodePath]
GO
ALTER TABLE [hierarchy].[NodePathNode] DROP CONSTRAINT [FK_NodePathNode_Node]
GO
ALTER TABLE [hierarchy].[NodePathDisplay] DROP CONSTRAINT [FK_NodePathDisplay_NodePath]
GO
ALTER TABLE [hierarchy].[NodePath] DROP CONSTRAINT [FK_NodePath_Node]
GO
ALTER TABLE [hierarchy].[NodePath] DROP CONSTRAINT [FK_NodePath_CourseNode]
GO
ALTER TABLE [hierarchy].[NodePath] DROP CONSTRAINT [FK_NodePath_CatalogueNode]
GO
ALTER TABLE [hierarchy].[NodeLink] DROP CONSTRAINT [FK_NodeLink_ParentNode]
GO
ALTER TABLE [hierarchy].[NodeLink] DROP CONSTRAINT [FK_NodeLink_ChildNode]
GO
ALTER TABLE [hierarchy].[Node] DROP CONSTRAINT [FK_Node_Type]
GO
ALTER TABLE [hierarchy].[Node] DROP CONSTRAINT [FK_Node_CurrentNodeVersion]
GO
/****** Object:  Table [hierarchy].[NodeVersion]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[NodeVersion]
GO
/****** Object:  Table [hierarchy].[NodeType]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[NodeType]
GO
/****** Object:  Table [hierarchy].[NodePathResource]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[NodePathResource]
GO
/****** Object:  Table [hierarchy].[NodePathNode]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[NodePathNode]
GO
/****** Object:  Table [hierarchy].[NodePathDisplay]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[NodePathDisplay]
GO
/****** Object:  Table [hierarchy].[NodePath]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[NodePath]
GO
/****** Object:  Table [hierarchy].[NodeLink]    Script Date: 17/02/2020 10:01:11 ******/
ALTER TABLE [hierarchy].[NodeLink] SET ( SYSTEM_VERSIONING = OFF  )
GO
/****** Object:  Table [hierarchy].[NodeLink]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[NodeLink]
GO
/****** Object:  Table [hierarchy].[Node]    Script Date: 17/02/2020 10:01:11 ******/
ALTER TABLE [hierarchy].[Node] SET ( SYSTEM_VERSIONING = OFF  )
GO
/****** Object:  Table [hierarchy].[Node]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[Node]
GO
/****** Object:  Table [hierarchy].[NodeResource]    Script Date: 17/02/2020 10:01:11 ******/
ALTER TABLE [hierarchy].[NodeResource] SET ( SYSTEM_VERSIONING = OFF  )
GO
/****** Object:  Table [hierarchy].[NodeResource]    Script Date: 17/02/2020 10:01:11 ******/
DROP TABLE [hierarchy].[NodeResource]
GO
/****** Object:  Table [hierarchy].[NodeResource]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hierarchy].[NodeResource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeId] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[DisplayOrder] [int] NULL,
	[VersionStatusId] [int] NOT NULL,
	[PublicationId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[VersionStartTime] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[VersionEndTime] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodeResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
),
	PERIOD FOR SYSTEM_TIME ([VersionStartTime], [VersionEndTime])
)
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hierarchy].[NodeResourceHistory] )
)
GO
/****** Object:  Table [hierarchy].[Node]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hierarchy].[Node](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeTypeId] [int] NOT NULL,
	[CurrentNodeVersionId] [int] NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](512) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[VersionStartTime] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[VersionEndTime] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_Hierarchy_Node] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
),
	PERIOD FOR SYSTEM_TIME ([VersionStartTime], [VersionEndTime])
)
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hierarchy].[NodeHistory] )
)
GO
/****** Object:  Table [hierarchy].[NodeLink]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hierarchy].[NodeLink](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ParentNodeId] [int] NOT NULL,
	[ChildNodeId] [int] NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[VersionStartTime] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[VersionEndTime] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodeLink] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
),
	PERIOD FOR SYSTEM_TIME ([VersionStartTime], [VersionEndTime])
)
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hierarchy].[NodeLinkHistory] )
)
GO
/****** Object:  Table [hierarchy].[NodePath]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hierarchy].[NodePath](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeId] [int] NOT NULL,
	[NodePath] [nvarchar](256) NOT NULL,
	[CatalogueNodeId] [int] NULL,
	[CourseNodeId] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodePath] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO
/****** Object:  Table [hierarchy].[NodePathDisplay]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
/****** Object:  Table [hierarchy].[NodePathNode]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hierarchy].[NodePathNode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodePathId] [int] NOT NULL,
	[NodeId] [int] NOT NULL,
	[Depth] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodePathNode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO
/****** Object:  Table [hierarchy].[NodePathResource]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hierarchy].[NodePathResource](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodePathId] [int] NOT NULL,
	[ResourceId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodePathResource] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO
/****** Object:  Table [hierarchy].[NodeType]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hierarchy].[NodeType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](512) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_NodeType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO
/****** Object:  Table [hierarchy].[NodeVersion]    Script Date: 17/02/2020 10:01:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hierarchy].[NodeVersion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeId] [int] NOT NULL,
	[VersionStatusId] [int] NOT NULL,
	[PublicationId] [int] NULL,
	[MajorVersion] [int] NULL,
	[MinorVersion] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_Version] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO
ALTER TABLE [hierarchy].[Node]  WITH CHECK ADD  CONSTRAINT [FK_Node_CurrentNodeVersion] FOREIGN KEY([CurrentNodeVersionId])
REFERENCES [hierarchy].[NodeVersion] ([Id])
GO
ALTER TABLE [hierarchy].[Node] CHECK CONSTRAINT [FK_Node_CurrentNodeVersion]
GO
ALTER TABLE [hierarchy].[Node]  WITH CHECK ADD  CONSTRAINT [FK_Node_Type] FOREIGN KEY([NodeTypeId])
REFERENCES [hierarchy].[NodeType] ([Id])
GO
ALTER TABLE [hierarchy].[Node] CHECK CONSTRAINT [FK_Node_Type]
GO
ALTER TABLE [hierarchy].[NodeLink]  WITH CHECK ADD  CONSTRAINT [FK_NodeLink_ChildNode] FOREIGN KEY([ChildNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO
ALTER TABLE [hierarchy].[NodeLink] CHECK CONSTRAINT [FK_NodeLink_ChildNode]
GO
ALTER TABLE [hierarchy].[NodeLink]  WITH CHECK ADD  CONSTRAINT [FK_NodeLink_ParentNode] FOREIGN KEY([ParentNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO
ALTER TABLE [hierarchy].[NodeLink] CHECK CONSTRAINT [FK_NodeLink_ParentNode]
GO
ALTER TABLE [hierarchy].[NodePath]  WITH CHECK ADD  CONSTRAINT [FK_NodePath_CatalogueNode] FOREIGN KEY([CatalogueNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO
ALTER TABLE [hierarchy].[NodePath] CHECK CONSTRAINT [FK_NodePath_CatalogueNode]
GO
ALTER TABLE [hierarchy].[NodePath]  WITH CHECK ADD  CONSTRAINT [FK_NodePath_CourseNode] FOREIGN KEY([CourseNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO
ALTER TABLE [hierarchy].[NodePath] CHECK CONSTRAINT [FK_NodePath_CourseNode]
GO
ALTER TABLE [hierarchy].[NodePath]  WITH CHECK ADD  CONSTRAINT [FK_NodePath_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO
ALTER TABLE [hierarchy].[NodePath] CHECK CONSTRAINT [FK_NodePath_Node]
GO
ALTER TABLE [hierarchy].[NodePathDisplay]  WITH CHECK ADD  CONSTRAINT [FK_NodePathDisplay_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO
ALTER TABLE [hierarchy].[NodePathDisplay] CHECK CONSTRAINT [FK_NodePathDisplay_NodePath]
GO
ALTER TABLE [hierarchy].[NodePathNode]  WITH CHECK ADD  CONSTRAINT [FK_NodePathNode_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO
ALTER TABLE [hierarchy].[NodePathNode] CHECK CONSTRAINT [FK_NodePathNode_Node]
GO
ALTER TABLE [hierarchy].[NodePathNode]  WITH CHECK ADD  CONSTRAINT [FK_NodePathNode_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO
ALTER TABLE [hierarchy].[NodePathNode] CHECK CONSTRAINT [FK_NodePathNode_NodePath]
GO
ALTER TABLE [hierarchy].[NodePathResource]  WITH CHECK ADD  CONSTRAINT [FK_NodePathResource_NodePath] FOREIGN KEY([NodePathId])
REFERENCES [hierarchy].[NodePath] ([Id])
GO
ALTER TABLE [hierarchy].[NodePathResource] CHECK CONSTRAINT [FK_NodePathResource_NodePath]
GO
ALTER TABLE [hierarchy].[NodePathResource]  WITH CHECK ADD  CONSTRAINT [FK_NodePathResource_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id])
GO
ALTER TABLE [hierarchy].[NodePathResource] CHECK CONSTRAINT [FK_NodePathResource_Resource]
GO
ALTER TABLE [hierarchy].[NodeResource]  WITH CHECK ADD  CONSTRAINT [FK_NodeResource_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO
ALTER TABLE [hierarchy].[NodeResource] CHECK CONSTRAINT [FK_NodeResource_Node]
GO
ALTER TABLE [hierarchy].[NodeResource]  WITH CHECK ADD  CONSTRAINT [FK_NodeResource_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id])
GO
ALTER TABLE [hierarchy].[NodeResource] CHECK CONSTRAINT [FK_NodeResource_Resource]
GO
ALTER TABLE [hierarchy].[NodeVersion]  WITH CHECK ADD  CONSTRAINT [FK_NodeVersion_Node] FOREIGN KEY([NodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO
ALTER TABLE [hierarchy].[NodeVersion] CHECK CONSTRAINT [FK_NodeVersion_Node]
GO