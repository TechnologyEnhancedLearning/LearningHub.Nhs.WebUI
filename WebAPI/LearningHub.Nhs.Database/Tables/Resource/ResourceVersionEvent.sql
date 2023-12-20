CREATE TABLE [resources].[ResourceVersionEvent](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ResourceVersionId] [int] NOT NULL,
	[ResourceVersionEventTypeId] [int] NOT NULL,
	[Details] [nvarchar](1024) NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Resources_VersionEvent] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) 
GO

ALTER TABLE [resources].[ResourceVersionEvent]  WITH CHECK ADD  CONSTRAINT [FK_ResourceVersionEvent_ResourceVersion] FOREIGN KEY([ResourceVersionId])
REFERENCES [resources].[ResourceVersion] ([Id])
GO

ALTER TABLE [resources].[ResourceVersionEvent] CHECK CONSTRAINT [FK_ResourceVersionEvent_ResourceVersion]
GO

CREATE INDEX IX_ResourceVersionEvent_ResourceVersion ON [resources].[ResourceVersionEvent](ResourceVersionId) WITH FILLFACTOR=95
GO