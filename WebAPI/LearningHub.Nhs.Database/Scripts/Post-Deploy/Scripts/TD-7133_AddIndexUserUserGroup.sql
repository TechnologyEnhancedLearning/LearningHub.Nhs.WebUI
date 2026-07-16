IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name='IX_UserUserGroup_UserId_Deleted' AND object_id = OBJECT_ID('[hub].[UserUserGroup]'))
BEGIN
CREATE INDEX IX_UserUserGroup_UserId_Deleted
ON hub.UserUserGroup (UserId, Deleted)
INCLUDE (UserGroupId) WITH (FILLFACTOR = 95);
END