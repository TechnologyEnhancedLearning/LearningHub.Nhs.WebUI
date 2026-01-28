CREATE TABLE [dbo].[lakeflowCaptureInstanceInfo_1_2](
	[oldCaptureInstance] [varchar](max) NULL,
	[newCaptureInstance] [varchar](max) NULL,
	[schemaName] [varchar](100) NOT NULL,
	[tableName] [varchar](255) NOT NULL,
	[committedCursor] [varchar](max) NULL,
	[triggerReinit] [bit] NULL,
 CONSTRAINT [replicantCaptureInstanceInfoPrimaryKey] PRIMARY KEY CLUSTERED 
(
	[schemaName] ASC,
	[tableName] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
