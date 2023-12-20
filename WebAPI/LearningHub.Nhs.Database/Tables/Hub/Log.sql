CREATE TABLE [hub].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](50) NOT NULL,
	[Logged] [datetime] NOT NULL,
	[Level] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Logger] [nvarchar](250) NULL,
	[Callsite] [nvarchar](max) NULL,
	[Exception] [nvarchar](max) NULL,
 [UserId] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

CREATE NONCLUSTERED INDEX [IX_Hub_UserId]
    ON [hub].[Log]([UserId] ASC) WITH (FILLFACTOR = 95);
GO

CREATE NONCLUSTERED INDEX [IX_Hub_Logged]
    ON [hub].[Log]([Logged] DESC) WITH (FILLFACTOR = 95);

