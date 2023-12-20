CREATE TABLE [resources].[ResourceAzureMediaAsset](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[FilePath] [nvarchar](1024) NOT NULL,
	[LocatorUri] [nvarchar](1024) NOT NULL,
	[EncodeJobName] [nvarchar](128) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_resource.ResourceAzureMediaAsset] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) 
GO

