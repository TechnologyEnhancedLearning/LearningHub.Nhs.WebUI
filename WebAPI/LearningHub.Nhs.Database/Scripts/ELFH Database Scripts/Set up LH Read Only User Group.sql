SET IDENTITY_INSERT  [dbo].[userGroupTBL] ON

INSERT INTO [dbo].[userGroupTBL]
           (userGroupId,userGroupTypeId,[userGroupName],[userGroupDescription],[userGroupCode],[emailRegExp],[deleted],[amendUserId],[amendDate])
SELECT 3066, 12, 'Learning Hub Read Only', 'Learning Hub Read Only', NULL, NULL, 0, 4, sysdatetimeoffset()

SET IDENTITY_INSERT  [dbo].[userGroupTBL] OFF