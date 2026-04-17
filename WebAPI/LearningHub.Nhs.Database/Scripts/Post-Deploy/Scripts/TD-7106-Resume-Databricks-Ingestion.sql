IF EXISTS (
    SELECT 1
    FROM sys.change_tracking_databases
    WHERE database_id = DB_ID()
)
BEGIN
    PRINT 'Change Tracking is enabled. Executing setup...';

    EXEC dbo.lakeflowSetupChangeTracking
    @Tables = 'activity.ResourceActivity,resources.VersionStatus,resources.ResourceType,activity.ScormActivity,activity.ActivityStatus,activity.ScormActivityInteraction,activity.ScormActivityInteractionCorrectResponse,resources.VideoResourceVersion,hierarchy.CatalogueNodeVersionProvider,resources.ResourceVersionKeyword,hierarchy.CatalogueNodeVersion,activity.MediaResourceActivity,resources.ResourceVersionAuthor,resources.ResourceVersionEvent,hub.UserProvider,resources.ResourceAccessibility,hierarchy.NodePath,resources.ResourceVersionRating,resources.ResourceVersionEventType,hierarchy.NodeType,hub.User,hierarchy.NodePathNode,hierarchy.VersionStatus,hub.UserProfile,hierarchy.NodeVersion,hierarchy.Publication,hub.Attribute,hierarchy.NodeResource,hierarchy.NodeLink,resources.Resource,hub.RoleUserGroup,hierarchy.CatalogueNodeVersionKeyword,hub.UserGroup,hub.UserUserGroup,resources.WebLinkResourceVersion,resources.ResourceReference,hierarchy.FolderNodeVersion,hub.Role,hub.AttributeType,activity.AssessmentResourceActivity,hub.Scope,resources.AssessmentResourceVersion,hub.UserGroupAttribute,hierarchy.Node,resources.ResourceVersion',
    @User = 'Elfhadmin',
    @Retention = '2 DAYS';
END
ELSE
BEGIN
    PRINT 'Change Tracking is NOT enabled on this database. Skipping execution.';
END