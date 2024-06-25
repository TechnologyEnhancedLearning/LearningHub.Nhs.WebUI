
CREATE INDEX IX_Resource_CurrentResourceVersionId ON resources.Resource (CurrentResourceVersionId)
GO
------------------------------
CREATE INDEX IX_Resource_ResourceTypeId ON resources.Resource (ResourceTypeId)
GO
------------------------------
CREATE INDEX IX_ResourceVersion_ResourceId ON resources.ResourceVersion (ResourceId)
GO
-----------------------------
CREATE INDEX IX_ResourceActivity_UserId_ActivityStatusId ON activity.ResourceActivity (UserId, ActivityStatusId)
GO
-------------------------------
CREATE INDEX IX_NodePath_CatalogueNodeId_Deleted ON hierarchy.NodePath (CatalogueNodeId, Deleted)
GO
-------------------------------
CREATE INDEX IX_UserBookmark_UserId_ResourceReferenceId_Deleted ON hub.UserBookmark (UserId, ResourceReferenceId, Deleted)
GO
