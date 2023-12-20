CREATE TABLE [resources].[ResourceTypeValidationRule](
	[Id] [int] NOT NULL,
	[ResourceTypeId] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_ValidationRule] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO