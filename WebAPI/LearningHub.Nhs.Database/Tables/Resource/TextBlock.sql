CREATE TABLE [resources].[TextBlock]
(
	[Id] [int] IDENTITY(1,1) NOT NULL, 
    [BlockId] [int] NOT NULL, 
    [Content] [nvarchar](MAX) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_TextBlock] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ),
)
GO

ALTER TABLE [resources].[TextBlock] WITH CHECK ADD CONSTRAINT [FK_TextBlock_BlockId] FOREIGN KEY([BlockId])
REFERENCES [resources].[Block] ([Id])
GO

ALTER TABLE [resources].[TextBlock] CHECK CONSTRAINT [FK_TextBlock_BlockId]
GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_TextBlock_BlockId]
    ON [resources].[TextBlock]([BlockId] ASC);
GO
