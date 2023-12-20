CREATE TABLE [resources].[WholeSlideImage]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](1000) NULL,
    [FileId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_WholeSlideImage] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[WholeSlideImage] WITH CHECK ADD CONSTRAINT [FK_WholeSlideImage_FileId] FOREIGN KEY([FileId])
REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[WholeSlideImage] CHECK CONSTRAINT [FK_WholeSlideImage_FileId]
GO
