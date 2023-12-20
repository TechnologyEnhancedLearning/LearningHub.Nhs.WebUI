CREATE TABLE [messaging].[EmailTemplate]
(
	[Id] INT NOT NULL,
	[LayoutId] INT NOT NULL,
	[Title] NVARCHAR(100) NULL,
	[Subject] NVARCHAR(MAX) NOT NULL,
	[Body] NVARCHAR(MAX) NOT NULL,
	[AvailableTags] NVARCHAR(MAX) NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NULL,
	[AmendDate] [datetimeoffset](7) NULL,
	CONSTRAINT [PK_email_template] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) ON [PRIMARY]
)
GO

ALTER TABLE [messaging].[EmailTemplate]  WITH CHECK ADD CONSTRAINT [FK_EmailTemplate_EmailTemplateLayout] FOREIGN KEY([LayoutId])
REFERENCES [messaging].[EmailTemplateLayout] ([Id])
GO

ALTER TABLE [messaging].[EmailTemplate] CHECK CONSTRAINT [FK_EmailTemplate_EmailTemplateLayout]
GO