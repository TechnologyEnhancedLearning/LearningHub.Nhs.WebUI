/****** Object:  Schema [hub]    Script Date: 21/02/2023 14:08:31 ******/
CREATE SCHEMA [hub]
GO
/****** Object:  Table [hub].[Log]    Script Date: 21/02/2023 14:08:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [hub].[Log](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Application] [nvarchar](50) NOT NULL,
	[Logged] [datetime] NOT NULL,
	[Level] [nvarchar](50) NOT NULL,
	[Message] [nvarchar](max) NOT NULL,
	[Logger] [nvarchar](250) NULL,
	[Callsite] [nvarchar](max) NULL,
	[Exception] [nvarchar](max) NULL,
	[UserId] [int] NOT NULL,
	[Username] [nvarchar](50) NULL,
 CONSTRAINT [PK_dbo.Log] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
) 
GO

ALTER TABLE [hub].[Log] ADD  DEFAULT ((0)) FOR [UserId]
GO
/****** Object:  StoredProcedure [hub].[InsertLog]    Script Date: 21/02/2023 14:08:31 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [hub].[InsertLog] 
(
	@Application varchar(50),
	@Logged datetime,
	@Level nvarchar(50),
	@Message nvarchar(max),
	@Logger nvarchar(250),
	@CallSite nvarchar(max),
	@Exception nvarchar(max),
	@UserId int = 0,
	@UserName nvarchar(50) = null
)
as

INSERT INTO [hub].[Log]
           ([Application]
           ,[Logged]
           ,[Level]
           ,[Message]
           ,[Logger]
           ,[Callsite]
           ,[Exception]
		   ,[UserId]
		   ,[UserName]
		   )
VALUES
(	
	@Application,
	@Logged,
	@Level,
	@Message,
	@Logger,
	@CallSite,
	@Exception,
	@UserId,
	@UserName
)
GO
