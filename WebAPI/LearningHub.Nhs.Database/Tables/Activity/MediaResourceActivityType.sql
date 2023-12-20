CREATE TABLE [activity].[MediaResourceActivityType](
	[Id] [int] NOT NULL,
	[Name] [nvarchar](128) NOT NULL,
	[CreateUserID] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserID] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
	[Deleted] [bit] NOT NULL,
 CONSTRAINT [PK_MediaResourceActivityType] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) ON [PRIMARY]
GO

ALTER TABLE [activity].[MediaResourceActivityType]  WITH CHECK ADD  CONSTRAINT [FK_MediaResourceActivityType_CreateUser] FOREIGN KEY([CreateUserID])
REFERENCES [hub].[User] ([Id])
GO
ALTER TABLE [activity].[MediaResourceActivityType] CHECK CONSTRAINT [FK_MediaResourceActivityType_CreateUser]
GO
ALTER TABLE [activity].[MediaResourceActivityType]  WITH CHECK ADD  CONSTRAINT [FK_MediaResourceActivityType_AmendUser] FOREIGN KEY([AmendUserID])
REFERENCES [hub].[User] ([Id])
GO
ALTER TABLE [activity].[MediaResourceActivityType] CHECK CONSTRAINT [FK_MediaResourceActivityType_AmendUser]
GO
