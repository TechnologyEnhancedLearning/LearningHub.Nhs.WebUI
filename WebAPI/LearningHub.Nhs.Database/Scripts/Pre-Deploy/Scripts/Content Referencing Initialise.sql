IF EXISTS (SELECT * from hierarchy.HierarchyEdit WHERE HierarchyEditStatusId = 1)-- Draft
BEGIN
    RAISERROR ('Cannot delete hierarchy.RootNodeId column with an edit in Draft status', 16, 1)
    RETURN
END

-- delete column RootNodeId from hierarchy.HierarchyEdit if it exists
IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'HierarchyEdit' AND COLUMN_NAME = 'RootNodeId')
BEGIN
    ALTER TABLE hierarchy.HierarchyEdit DROP CONSTRAINT FK_HierarchyEdit_Node
    ALTER TABLE hierarchy.HierarchyEdit DROP COLUMN RootNodeId
END


-- Add record to table hierarchy.HierarchyEditDetailOperation if it doesn't already exist:
IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailOperation] WHERE [Id] = 4)
BEGIN
    INSERT INTO [hierarchy].[HierarchyEditDetailOperation] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
    VALUES (4,N'Add Reference',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END
