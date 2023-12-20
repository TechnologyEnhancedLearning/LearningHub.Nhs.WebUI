CREATE TABLE [hub].[Address](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Address1] [nvarchar](100) NULL,
	[Address2] [nvarchar](100) NULL,
	[Address3] [nvarchar](100) NULL,
	[Address4] [nvarchar](100) NULL,
	[Town] [nvarchar](100) NULL,
	[County] [nvarchar](100) NULL,
	[PostCode] [nvarchar](8) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO