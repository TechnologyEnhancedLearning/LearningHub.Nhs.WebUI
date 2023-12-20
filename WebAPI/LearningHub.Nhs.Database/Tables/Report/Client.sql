CREATE TABLE [reports].[Client]
(
	[Id] INT NOT NULL,
	[ClientId] UNIQUEIDENTIFIER NOT NULL,
	[Name] NVARCHAR(128) NOT NULL,	
	[Deleted] [bit] NOT NULL,
	[CreateUserId] INT NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] INT NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	CONSTRAINT [PK_Reports_Client] PRIMARY KEY CLUSTERED 
	(
		[Id] ASC
	) ON [PRIMARY]
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IX_Reports_Client] ON [reports].[Client]
(
	[ClientId] ASC
) ON [PRIMARY]
GO