CREATE TABLE [resources].[Attachment]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [FileId] [int] NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_Attachment] PRIMARY KEY CLUSTERED
        (
         [Id] ASC
            ),
)
GO

ALTER TABLE [resources].[Attachment] WITH CHECK ADD CONSTRAINT [FK_Attachment_FileId] FOREIGN KEY([FileId])
    REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[Attachment] CHECK CONSTRAINT [FK_Attachment_FileId]
GO 
