CREATE TABLE [resources].[Video]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [FileId] [int] NOT NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_Video] PRIMARY KEY CLUSTERED
        ([Id] ASC),
)
GO

ALTER TABLE [resources].[Video] WITH CHECK ADD CONSTRAINT [FK_Video_FileId] FOREIGN KEY([FileId])
    REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[Video] CHECK CONSTRAINT [FK_Video_FileId]
GO
