CREATE TABLE [hub].[RoadmapType](
	[Id] [int] IDENTITY(1, 1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT(0),
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_RoadmapType] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	)
)