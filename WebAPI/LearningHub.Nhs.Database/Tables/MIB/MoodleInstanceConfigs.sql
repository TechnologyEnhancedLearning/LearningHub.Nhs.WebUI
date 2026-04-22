CREATE TABLE [dbo].[MoodleInstanceConfigs](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[BaseUrl] [nvarchar](500) NOT NULL,
	[ShortName] [nvarchar](100) NOT NULL,
	[TokenSecretName] [nvarchar](200) NOT NULL,
	[EnabledEndpoints] [nvarchar](1000) NULL,
	[IsEnabled] [bit] NOT NULL,
	[Weighting] [int] NOT NULL,
	[CreatedAt] [datetime2](7) NOT NULL,
	[UpdatedAt] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_InstanceConfigs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MoodleInstanceConfigs] ADD  CONSTRAINT [DF_InstanceConfigs_IsEnabled]  DEFAULT ((1)) FOR [IsEnabled]
GO

ALTER TABLE [dbo].[MoodleInstanceConfigs] ADD  CONSTRAINT [DF_InstanceConfigs_Weighting]  DEFAULT ((100)) FOR [Weighting]
GO

ALTER TABLE [dbo].[MoodleInstanceConfigs] ADD  CONSTRAINT [DF_InstanceConfigs_CreatedAt]  DEFAULT (sysutcdatetime()) FOR [CreatedAt]
GO

ALTER TABLE [dbo].[MoodleInstanceConfigs] ADD  CONSTRAINT [DF_InstanceConfigs_UpdatedAt]  DEFAULT (sysutcdatetime()) FOR [UpdatedAt]
GO