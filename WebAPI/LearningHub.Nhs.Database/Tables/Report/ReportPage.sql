CREATE TABLE [reports].[ReportPage]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[Html] NTEXT NOT NULL,	
	[ReportId] INT NOT NULL,	
	[ReportOrientationModeId] INT NOT NULL,	
	[Deleted] [bit] NOT NULL,
	[CreateUserId] INT NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] INT NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_Reports_ReportPage] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) ON [PRIMARY]
)

GO

ALTER TABLE [reports].[ReportPage] WITH CHECK ADD CONSTRAINT [FK_ReportPage_Report] FOREIGN KEY([ReportId])
REFERENCES [reports].[Report] ([Id])
GO

ALTER TABLE [reports].[ReportPage] CHECK CONSTRAINT [FK_ReportPage_Report]
GO

ALTER TABLE [reports].[ReportPage] WITH CHECK ADD CONSTRAINT [FK_ReportPage_ReportOrientationMode] FOREIGN KEY([ReportOrientationModeId])
REFERENCES [reports].[ReportOrientationMode] ([Id])
GO

ALTER TABLE [reports].[ReportPage] CHECK CONSTRAINT [FK_ReportPage_ReportOrientationMode]
GO