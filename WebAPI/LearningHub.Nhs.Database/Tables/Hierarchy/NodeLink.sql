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
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hierarchy].[NodeLinkHistory] )
)
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

CREATE INDEX IX_NodeLink_ChildNodeId ON [hierarchy].[NodeLink](ChildNodeId)
WITH (FILLFACTOR = 95);
GO

CREATE INDEX IX_NodeLink_ParentNodeId ON [hierarchy].[NodeLink](ParentNodeId)
WITH (FILLFACTOR = 95);
GO
