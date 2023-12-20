CREATE TABLE [reports].[Report]
(
	[Id] INT NOT NULL IDENTITY (1, 1),
	[Name] NVARCHAR(128) NOT NULL,
	[ClientId] INT NOT NULL,
	[FileName] NVARCHAR(512) NULL,	
	[Hash] NVARCHAR(64) NULL,	
	[ReportTypeId] int,
	[ReportStatusId] int,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] INT NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] INT NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_Reports_Report] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) ON [PRIMARY]
)

GO

ALTER TABLE [reports].[Report] WITH CHECK ADD CONSTRAINT [FK_Report_ReportType] FOREIGN KEY([ReportTypeId])
REFERENCES [reports].[ReportType] ([Id])
GO

ALTER TABLE [reports].[Report] CHECK CONSTRAINT [FK_Report_ReportType]
GO

ALTER TABLE [reports].[Report] WITH CHECK ADD CONSTRAINT [FK_Report_ReportStatus] FOREIGN KEY([ReportStatusId])
REFERENCES [reports].[ReportStatus] ([Id])
GO

ALTER TABLE [reports].[Report] CHECK CONSTRAINT [FK_Report_ReportStatus]
GO

ALTER TABLE [reports].[Report] WITH CHECK ADD CONSTRAINT [FK_Report_Client] FOREIGN KEY([ClientId])
REFERENCES [reports].[Client] ([Id])
GO

ALTER TABLE [reports].[Report] CHECK CONSTRAINT [FK_Report_Client]
GO