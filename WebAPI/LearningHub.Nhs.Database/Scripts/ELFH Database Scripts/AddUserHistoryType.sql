IF NOT EXISTS(SELECT 1 FROM [dbo].[userHistoryTypeTBL] Where UserHistoryTypeId = 13)
BEGIN
   INSERT INTO [dbo].[userHistoryTypeTBL]
           ([UserHistoryTypeId]
           ,[Description])
     VALUES
           (13
           ,'Log in using user name')
END


IF NOT EXISTS(SELECT 1 FROM [dbo].[userHistoryTypeTBL] Where UserHistoryTypeId = 14)
BEGIN
   INSERT INTO [dbo].[userHistoryTypeTBL]
           ([UserHistoryTypeId]
           ,[Description])
     VALUES
           (14
           ,'Log in using email address')
END

-- userLoginTypeTBL

CREATE TABLE [dbo].[userLoginTypeTBL](
	[userLoginTypeId] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NOT NULL,
	[loginInfo] [nvarchar](100) NOT NULL,
	[deleted] [bit] NOT NULL,
	[createUserId] [int] NOT NULL,
	[createDate] [datetimeoffset](7) NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
	[userHistoryTypeId] [int] NOT NULL,
 CONSTRAINT [PK_userLoginType] PRIMARY KEY CLUSTERED 
(
	[userLoginTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[userLoginTypeTBL]  WITH CHECK ADD  CONSTRAINT [FK_userLoginTypeTBL_userHistoryTypeTBL] FOREIGN KEY([userHistoryTypeId])
REFERENCES [dbo].[userHistoryTypeTBL] ([UserHistoryTypeId])
GO

ALTER TABLE [dbo].[userLoginTypeTBL] CHECK CONSTRAINT [FK_userLoginTypeTBL_userHistoryTypeTBL]
GO

ALTER TABLE [dbo].[userLoginTypeTBL]  WITH CHECK ADD  CONSTRAINT [FK_userLoginTypeTBL_userTbl] FOREIGN KEY([userId])
REFERENCES [dbo].[userTBL] ([userId])
GO

ALTER TABLE [dbo].[userLoginTypeTBL] CHECK CONSTRAINT [FK_userLoginTypeTBL_userTbl]
GO




