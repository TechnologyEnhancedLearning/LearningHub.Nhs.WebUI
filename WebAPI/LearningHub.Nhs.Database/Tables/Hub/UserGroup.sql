CREATE TABLE [hub].[UserGroup](
	[Id] [int] IDENTITY(10000,1) NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Description] [varchar](255) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_userGroup] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[UserGroup] ADD  CONSTRAINT [DF_userGroup_amendDate]  DEFAULT (sysdatetimeoffset()) FOR [AmendDate]
GO

CREATE NONCLUSTERED INDEX IX_UserProfile_EmailAddress
    ON hub.[UserProfile] (EmailAddress);   
GO  

---Add extended property for table 
EXEC sys.sp_addextendedproperty @name=N'Description', @value=N'Table to define user group' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'UserGroup' 
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Technical Description', @value=N'User group name', @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'UserGroup', @level2type=N'COLUMN',@level2name=N'Name'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Business Description', @value=N'', @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'UserGroup', @level2type=N'COLUMN',@level2name=N'Name'
GO

-----Add extended property for column
EXEC sys.sp_addextendedproperty @name=N'Technical Description', @value=N'User group description to describe the functionality of the group', @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'UserGroup', @level2type=N'COLUMN',@level2name=N'Description'
GO

EXEC sys.sp_addextendedproperty @name=N'Business Description', @value=N'' , @level0type=N'SCHEMA',@level0name=N'hub', @level1type=N'TABLE',@level1name=N'UserGroup', @level2type=N'COLUMN',@level2name=N'Description'
GO
