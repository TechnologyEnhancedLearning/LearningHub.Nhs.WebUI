CREATE TABLE [external].[ExternalSystemDeepLink](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Code] [nvarchar](100) NOT NULL,
	[DeepLink] [nvarchar](100) NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NULL,
	[AmendDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_ExternalSystemDeepLink] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO