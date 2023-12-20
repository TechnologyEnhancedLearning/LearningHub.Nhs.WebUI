CREATE TABLE [messaging].[Message](
	[Id] [int] NOT NULL IDENTITY (1, 1),
	[Subject] [nvarchar](512) NULL,
	[Body] [nvarchar](max) NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_Message] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO
