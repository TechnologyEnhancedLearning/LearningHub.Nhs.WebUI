-- Add record to table hierarchy.HierarchyEditDetailOperation if it doesn't already exist:
IF NOT EXISTS (SELECT 1 FROM [hierarchy].[HierarchyEditDetailOperation] WHERE [Id] = 4)
BEGIN
    INSERT INTO [hierarchy].[HierarchyEditDetailOperation] ([Id],[Description],[Deleted],[AmendUserId],[AmendDate],[CreateUserId],[CreateDate])
    VALUES (4,N'Add Reference',0,4,SYSDATETIMEOFFSET(),4,SYSDATETIMEOFFSET())
END
