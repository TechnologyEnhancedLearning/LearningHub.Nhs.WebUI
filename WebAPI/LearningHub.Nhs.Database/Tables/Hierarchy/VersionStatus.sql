CREATE TABLE [hierarchy].[VersionStatus](
	[Id] [int] NOT NULL,
	[Description] [nvarchar](32) NULL,
	[Deleted] [bit] NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hierarchy_VersionStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO