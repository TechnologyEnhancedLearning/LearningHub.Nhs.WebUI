CREATE TABLE [messaging].[MessageType]
(
	[Id] INT NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
	 CONSTRAINT [PK_MessageType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO