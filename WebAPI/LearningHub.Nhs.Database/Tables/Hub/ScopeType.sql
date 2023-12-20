CREATE TABLE [hub].[ScopeType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](255) NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_ScopeType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) 
GO