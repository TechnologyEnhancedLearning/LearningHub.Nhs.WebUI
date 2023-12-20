CREATE TABLE [reports].[ReportStatus]
(
	[Id] INT NOT NULL,	
	[Name] NVARCHAR(128) NOT NULL,
	[Description] NVARCHAR(512) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] INT NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] INT NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_Reports_ReportStatus] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) ON [PRIMARY]
)
