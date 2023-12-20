CREATE TABLE [adf].[Staging_ElfhExternalSystemUser](
	[JobRunId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[ExternalSystemId] [int] NOT NULL,
	[UserName] [nvarchar](50) NOT NULL,
	[Code] [nvarchar](50) NOT NULL,
	[Active] [bit] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NULL,
	[AmendDate] [datetimeoffset](7) NULL
) ON [PRIMARY]