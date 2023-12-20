CREATE TABLE [activity].[MediaResourceActivityInteraction](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MediaResourceActivityId] [int] NOT NULL,
	[MediaResourceActivityTypeId] [int] NOT NULL,
	[ClientDateTime] [DateTimeOffset](7) NOT NULL,
	[DisplayTime] [time](7) NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_MediaResourceActivityInteraction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [activity].[MediaResourceActivityInteraction]  WITH CHECK ADD  CONSTRAINT [FK_MediaResourceActivity_AmendUser] FOREIGN KEY([AmendUserID])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[MediaResourceActivityInteraction] CHECK CONSTRAINT [FK_MediaResourceActivity_AmendUser]
GO

ALTER TABLE [activity].[MediaResourceActivityInteraction]  WITH CHECK ADD  CONSTRAINT [FK_MediaResourceActivity_CreateUser] FOREIGN KEY([CreateUserID])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [activity].[MediaResourceActivityInteraction] CHECK CONSTRAINT [FK_MediaResourceActivity_CreateUser]
GO
ALTER TABLE [activity].[MediaResourceActivityInteraction]  WITH CHECK ADD  CONSTRAINT [FK_MediaResourceActivity_MediaResourceActivity] FOREIGN KEY([MediaResourceActivityId])
REFERENCES [activity].[MediaResourceActivity] ([Id])
GO

ALTER TABLE [activity].[MediaResourceActivityInteraction] CHECK CONSTRAINT [FK_MediaResourceActivity_MediaResourceActivity]
GO

ALTER TABLE [activity].[MediaResourceActivityInteraction]  WITH CHECK ADD  CONSTRAINT [FK_MediaResourceActivity_MediaResourceActivityType] FOREIGN KEY([MediaResourceActivityTypeId])
REFERENCES [activity].[MediaResourceActivityType] ([Id])
GO

ALTER TABLE [activity].[MediaResourceActivityInteraction] CHECK CONSTRAINT [FK_MediaResourceActivity_MediaResourceActivityType]
GO

CREATE INDEX IX_MediaResourceActivityInteraction_MediaResourceActivity ON [activity].[MediaResourceActivityInteraction](MediaResourceActivityId)
WITH (FILLFACTOR = 95);
GO