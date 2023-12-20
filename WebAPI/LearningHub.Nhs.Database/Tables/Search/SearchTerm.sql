CREATE TABLE [analytics].[SearchTerm](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[SearchText] [nvarchar](255) NULL,	
	[SortColumn] [nvarchar](50) NULL,
	[SortDirection] [nvarchar](20) NULL,
	[NumberOfHits] int NULL,
	[TotalNumberOfHits] int NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_SearchTerm] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [analytics].[SearchTerm]  WITH CHECK ADD  CONSTRAINT [FK_SearchTerm_SearchTerm_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[SearchTerm] CHECK CONSTRAINT [FK_SearchTerm_SearchTerm_AmendUser]
GO

ALTER TABLE [analytics].[SearchTerm]  WITH CHECK ADD  CONSTRAINT [FK_SearchTerm_SearchTerm_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [analytics].[SearchTerm] CHECK CONSTRAINT [FK_SearchTerm_SearchTerm_CreateUser]
GO
