CREATE TABLE [activity].[ActivityStatus](
	[Id] [int] NOT NULL,
	[ActivityStatus] [nvarchar](50) NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_Activity_ActivityStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [activity].[ActivityStatus] ADD  DEFAULT ((0)) FOR [Deleted]
GO
ALTER TABLE [activity].[ActivityStatus] ADD  DEFAULT (sysdatetimeoffset()) FOR [AmendDate]
GO
ALTER TABLE [activity].[ActivityStatus]  WITH CHECK ADD  CONSTRAINT [FK_ActivityStatus_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO
ALTER TABLE [activity].[ActivityStatus] CHECK CONSTRAINT [FK_ActivityStatus_CreateUser]
GO
ALTER TABLE [activity].[ActivityStatus]  WITH CHECK ADD  CONSTRAINT [FK_ActivityStatus_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO
ALTER TABLE [activity].[ActivityStatus] CHECK CONSTRAINT [FK_ActivityStatus_AmendUser]
GO