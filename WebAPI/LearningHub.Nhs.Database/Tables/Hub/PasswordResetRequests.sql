
CREATE TABLE [hub].[PasswordResetRequests](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[EmailAddress] [nvarchar](100) NOT NULL,
	[RequestTime] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Hub_PasswordResetRequests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

