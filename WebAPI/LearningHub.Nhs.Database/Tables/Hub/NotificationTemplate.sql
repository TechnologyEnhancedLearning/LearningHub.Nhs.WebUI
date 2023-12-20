CREATE TABLE [hub].[NotificationTemplate]
(
	[Id] INT NOT NULL,
	[Title] NVARCHAR(100) NULL,
	[Subject] NVARCHAR(MAX) NOT NULL,
	[Body] NVARCHAR(MAX) NOT NULL,
	[AvailableTags] NVARCHAR(MAX) NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NULL,
	[AmendDate] [datetimeoffset](7) NULL,
	CONSTRAINT [PK_notification_template] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) ON [PRIMARY]
)
GO
