CREATE TABLE [resources].[WholeSlideImageBlock]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
    [BlockId] [int] NOT NULL, 
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_WholeSlideImageBlock] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[WholeSlideImageBlock] WITH CHECK ADD CONSTRAINT [FK_WholeSlideImageBlock_BlockId] FOREIGN KEY([BlockId])
REFERENCES [resources].[Block] ([Id])
GO

ALTER TABLE [resources].[WholeSlideImageBlock] CHECK CONSTRAINT [FK_WholeSlideImageBlock_BlockId]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_WholeSlideImageBlock_BlockId]
    ON [resources].[WholeSlideImageBlock]([BlockId] ASC);
GO
