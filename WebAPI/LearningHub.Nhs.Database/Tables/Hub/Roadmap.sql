CREATE TABLE [hub].[Roadmap](
	[Id] [int] IDENTITY(1, 1) NOT NULL,
	[RoadmapTypeId] [int] NOT NULL,
	[Title] [varchar](200) NOT NULL,
	[Description] [varchar](max) NOT NULL,
	[RoadmapDate] [datetimeoffset](7) NULL,
	[ImageName] [varchar](100) NULL,
	[OrderNumber] [int] NOT NULL,
	[Published] [bit] NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT(0),
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_Roadmap] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)
GO

ALTER TABLE [hub].[Roadmap] WITH CHECK ADD CONSTRAINT [FK_roadmap_roadmapType] FOREIGN KEY([RoadmapTypeId])
REFERENCES [hub].[RoadmapType] ([Id])
GO

ALTER TABLE [hub].[Roadmap] CHECK CONSTRAINT [FK_roadmap_roadmapType]
GO