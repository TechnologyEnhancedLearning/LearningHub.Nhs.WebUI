CREATE TABLE [hierarchy].[Node](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[NodeTypeId] [int] NOT NULL,
	[CurrentNodeVersionId] [int] NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](4000) NOT NULL,
	[Hidden] [bit] NOT NULL DEFAULT 0,
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
) ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON ( HISTORY_TABLE = [hierarchy].[NodeHistory] )
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
