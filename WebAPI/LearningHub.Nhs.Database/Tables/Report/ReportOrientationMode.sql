CREATE TABLE [reports].[ReportOrientationMode]
(
	[Id] INT NOT NULL,	
	[Name] NVARCHAR(20) NOT NULL,	
	[Deleted] [bit] NOT NULL,
	[CreateUserId] INT NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] INT NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_Reports_ReportOrientationMode] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) ON [PRIMARY]
)
