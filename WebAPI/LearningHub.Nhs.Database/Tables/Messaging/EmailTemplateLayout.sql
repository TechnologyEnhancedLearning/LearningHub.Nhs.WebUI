CREATE TABLE [messaging].[EmailTemplateLayout]
(
	[Id] INT NOT NULL,
	[Name] NVARCHAR(100) NOT NULL,
	[Body] NVARCHAR(MAX) NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NULL,
	[AmendDate] [datetimeoffset](7) NULL,
	CONSTRAINT [PK_email_template_layout] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) ON [PRIMARY]
)
