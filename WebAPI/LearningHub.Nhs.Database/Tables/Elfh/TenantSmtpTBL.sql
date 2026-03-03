CREATE TABLE [elfh].[tenantSmtpTBL](
	[tenantId] [int] NOT NULL,
	[deliveryMethod] [nvarchar](128) NOT NULL,
	[pickupDirectoryLocation] [nvarchar](256) NULL,
	[from] [nvarchar](256) NULL,
	[userName] [nvarchar](256) NULL,
	[password] [nvarchar](256) NULL,
	[enableSsl] [bit] NULL,
	[host] [nvarchar](256) NULL,
	[port] [int] NULL,
	[active] [bit] NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL
) ON [PRIMARY]
GO
