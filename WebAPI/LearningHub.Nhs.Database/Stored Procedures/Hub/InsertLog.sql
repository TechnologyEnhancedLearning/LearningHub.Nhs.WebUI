CREATE procedure [hub].[InsertLog] 
(
	@Application varchar(50),
	@Logged datetime,
	@Level nvarchar(50),
	@Message nvarchar(max),
	@Logger nvarchar(250),
	@CallSite nvarchar(max),
	@Exception nvarchar(max),
	@UserId int
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
		   ,[UserId])
VALUES
(	
	@Application,
	@Logged,
	@Level,
	@Message,
	@Logger,
	@CallSite,
	@Exception,
	@UserId
)
GO