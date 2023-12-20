CREATE TABLE [hub].[Permission](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](100) NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](255) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_permission] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [hub].[Permission] ADD  DEFAULT ((0)) FOR [Deleted]
GO

