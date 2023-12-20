CREATE TABLE [analytics].[SearchAction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SearchId] [int] NOT NULL,
	[ResourceId] [int] NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_SearchAction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [analytics].[SearchAction]  WITH CHECK ADD  CONSTRAINT [FK_SearchAction_SearchAction_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[SearchAction] CHECK CONSTRAINT [FK_SearchAction_SearchAction_AmendUser]
GO

ALTER TABLE [analytics].[SearchAction]  WITH CHECK ADD  CONSTRAINT [FK_SearchAction_SearchAction_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[SearchAction] CHECK CONSTRAINT [FK_SearchAction_SearchAction_CreateUser]
GO

ALTER TABLE [analytics].[SearchAction]  WITH CHECK ADD  CONSTRAINT [FK_SearchAction_SearchAction_SearchTerm] FOREIGN KEY([SearchId])
REFERENCES [analytics].[SearchTerm] ([Id])
GO

ALTER TABLE [analytics].[SearchAction] CHECK CONSTRAINT [FK_SearchAction_SearchAction_SearchTerm]
GO