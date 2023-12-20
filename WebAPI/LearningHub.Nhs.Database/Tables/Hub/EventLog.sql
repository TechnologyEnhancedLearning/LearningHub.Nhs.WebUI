CREATE TABLE [hub].[EventLog](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[Description] [nvarchar](128) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hub_EventLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

