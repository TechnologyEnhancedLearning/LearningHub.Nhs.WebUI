CREATE TABLE [resources].[ResourceLicence](
	[Id] [int] NOT NULL,
	[Title] [nvarchar](128) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ResourceLicence] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO
