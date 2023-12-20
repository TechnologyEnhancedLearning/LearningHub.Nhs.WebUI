CREATE TABLE [resources].[BlockCollection]
(
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
    CONSTRAINT [PK_Resources_BlockCollection] PRIMARY KEY CLUSTERED
    (
	    [Id] ASC
    ),
)
GO
