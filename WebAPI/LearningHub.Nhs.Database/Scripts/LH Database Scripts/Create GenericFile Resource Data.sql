-- create generic file type resource
DECLARE @resourceversionId int

SELECT @resourceversionId  = [Id] FROM [resources].[ResourceVersion] where Title = 'Test Generic File Resource'

If not exists(select Id from [resources].[File] where [FileName] ='edinburgh_scale.pdf') and @resourceversionId > 0

BEGIN

--create pdf entry
declare @fileTypeId int = 3;
declare @filename varchar(1000) = 'edinburgh_scale.pdf';
declare @filepath varchar(1000) = 'edinburgh_scale.pdf';
declare @filesize int = 39;
declare @userId int = 314282;
declare @fileId int


INSERT INTO [resources].[File]
           ([FileTypeId]
           ,[FileName]
           ,[FilePath]
           ,[FileSizeKb]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (@fileTypeId, @filename, @filepath, @filesize, 0, @userId, SYSDATETIMEOFFSET(), @userId, SYSDATETIMEOFFSET())

		   SELECT @fileId = SCOPE_IDENTITY()

		   INSERT INTO [resources].[GenericFileResourceVersion]
           ([ResourceVersionId]
           ,[FileId]
           ,[ImageContent]
           ,[VideoContent]
           ,[AudioContent]
           ,[DocumentContent]
           ,[SpreadsheetContent]
           ,[PresentationContent]
           ,[WeblnksContent]
           ,[ScormAiccContent]
           ,[OtherContent]
           ,[AuthoredYear]
           ,[AuthoredMonth]
           ,[AuthoredDayOfMonth]
           ,[NextReviewDate]
           ,[Deleted]
           ,[CreateUserId]
           ,[CreateDate]
           ,[AmendUserId]
           ,[AmendDate])
     VALUES
           (@resourceversionId
           ,@fileId 
           ,0
           ,0
           ,0
           ,1
           ,0
           ,0
           ,0
           ,0
           ,0
           ,2001
           ,1
           ,15
           ,null
           ,0
           ,@userId
           ,SYSDATETIMEOFFSET()
           ,@userId
           ,SYSDATETIMEOFFSET())


END
        


