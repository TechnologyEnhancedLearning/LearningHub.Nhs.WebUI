CREATE TABLE [hub].[DetectJsLog](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[JsEnabledRequest] [int] NOT NULL,
	[JsDisabledRequest] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
CONSTRAINT [PK_DetectJsLog] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO