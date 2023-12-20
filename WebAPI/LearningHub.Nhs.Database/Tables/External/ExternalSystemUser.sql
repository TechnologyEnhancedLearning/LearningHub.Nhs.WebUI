CREATE TABLE [external].[ExternalSystemUser](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] INT NOT NULL,
	[ExternalSystemId] INT NOT NULL,
	[Deleted] [bit] NOT NULL DEFAULT 0,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NULL,
	[AmendDate] [datetimeoffset](7) NULL,
 CONSTRAINT [PK_ExternalSystemUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [external].[ExternalSystemUser]  ADD  CONSTRAINT [FK_ExternalSystemUser_ExternalSystemId] FOREIGN KEY([ExternalSystemId])
REFERENCES [external].[ExternalSystem] ([Id])
GO