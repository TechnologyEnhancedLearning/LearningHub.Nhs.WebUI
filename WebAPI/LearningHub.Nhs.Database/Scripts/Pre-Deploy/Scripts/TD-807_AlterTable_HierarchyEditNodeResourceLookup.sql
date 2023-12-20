/*
	Tidy up & alter HierarchyEditNodeResourceLookup table
*/

-- Remove old temporary records or Published, Discarded and Failed Hierarchy Edits
DELETE FROM hed
FROM [hierarchy].[HierarchyEditDetail] hed
INNER JOIN [hierarchy].[HierarchyEdit] he ON hed.HierarchyEditId = he.Id
WHERE	he.HierarchyEditStatusId in (2 /* Published */, 3 /* Discarded */, 6 /* Failed to Publish */)


DELETE FROM henrl
FROM [hierarchy].[HierarchyEditNodeResourceLookup] henrl
INNER JOIN [hierarchy].[HierarchyEdit] he ON henrl.HierarchyEditId = he.Id
WHERE	he.HierarchyEditStatusId in (2 /* Published */, 3 /* Discarded */, 6 /* Failed to Publish */)
GO

-- Add Id column and index to HierarchyEditNodeResourceLookup table
CREATE TABLE [hierarchy].[HierarchyEditNodeResourceLookup_Temp](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[HierarchyEditId] [int] NOT NULL,
	[NodeId] [int] NOT NULL,
	[ResourceId] [int] NOT NULL
) ON [PRIMARY]
GO

INSERT INTO [hierarchy].[HierarchyEditNodeResourceLookup_Temp] (HierarchyEditId, NodeId, ResourceId)
SELECT	HierarchyEditId, NodeId, ResourceId
FROM	[hierarchy].[HierarchyEditNodeResourceLookup]
GO

IF EXISTS (SELECT OBJECT_ID('hierarchy.FK_HierarchyEditNodeResourceLookup_HierarchyEdit', 'F'))
ALTER TABLE [hierarchy].[HierarchyEditNodeResourceLookup] DROP CONSTRAINT [FK_HierarchyEditNodeResourceLookup_HierarchyEdit]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[hierarchy].[HierarchyEditNodeResourceLookup]') AND type in (N'U'))
DROP TABLE [hierarchy].[HierarchyEditNodeResourceLookup]
GO

EXEC sp_rename '[hierarchy].[HierarchyEditNodeResourceLookup_Temp]', 'HierarchyEditNodeResourceLookup'
GO

ALTER TABLE [hierarchy].[HierarchyEditNodeResourceLookup]  WITH CHECK ADD  CONSTRAINT [FK_HierarchyEditNodeResourceLookup_HierarchyEdit] FOREIGN KEY([HierarchyEditId])
REFERENCES [hierarchy].[HierarchyEdit] ([Id])
GO

ALTER TABLE [hierarchy].[HierarchyEditNodeResourceLookup] CHECK CONSTRAINT [FK_HierarchyEditNodeResourceLookup_HierarchyEdit]
GO

CREATE INDEX idx_HierarchyEditNodeResourceLookup_HierarchyEditId
ON [hierarchy].[HierarchyEditNodeResourceLookup] (HierarchyEditId)
GO
