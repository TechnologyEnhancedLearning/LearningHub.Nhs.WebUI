CREATE TABLE [hierarchy].[HierarchyEditNodeResourceLookup] (
    [Id]              INT IDENTITY (1, 1) NOT NULL,
    [HierarchyEditId] INT NOT NULL,
    [NodeId]          INT NOT NULL,
    [ResourceId]      INT NOT NULL,
    CONSTRAINT [FK_HierarchyEditNodeResourceLookup_HierarchyEdit] FOREIGN KEY ([HierarchyEditId]) REFERENCES [hierarchy].[HierarchyEdit] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [idx_HierarchyEditNodeResourceLookup_HierarchyEditId]
    ON [hierarchy].[HierarchyEditNodeResourceLookup]([HierarchyEditId] ASC);
GO

