CREATE TABLE [resources].[Image]
(
    [Id] [int] IDENTITY(1,1) NOT NULL,
    [FileId] [int] NULL,
    [AltText] [nvarchar](125) NULL,
    [Description] [nvarchar](250) NULL,
    [Deleted] [bit] NOT NULL,
    [CreateUserId] [int] NOT NULL,
    [CreateDate] [datetimeoffset](7) NOT NULL,
    [AmendUserId] [int] NOT NULL,
    [AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_Image] PRIMARY KEY CLUSTERED
        ([Id] ASC),
)
GO

ALTER TABLE [resources].[Image] WITH CHECK ADD CONSTRAINT [FK_Image_FileId] FOREIGN KEY([FileId])
    REFERENCES [resources].[File] ([Id])
GO

ALTER TABLE [resources].[Image] CHECK CONSTRAINT [FK_Image_FileId]
GO 
