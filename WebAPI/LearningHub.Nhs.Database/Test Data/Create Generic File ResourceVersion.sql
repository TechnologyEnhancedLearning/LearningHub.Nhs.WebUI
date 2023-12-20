--------------------------------------------------------
-- 1. Create a 'Draft' Generic File Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 9 -- Generic File
DECLARE @Title nvarchar(128) ='Test DRAFT Generic File Resource'
DECLARE @Description nvarchar(512) ='Test DRAFT Generic File Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'
DECLARE @FileTypeId int = 1 -- doc
DECLARE @FileName nvarchar(128)='Logon wizard Error Messages.docx'
DECLARE @FilePath nvarchar(1024) = '04af408b-1c2b-4e21-ae68-5da9c29cc601'
DECLARE @AuthorName nvarchar(50) = 'Generic File Author'
DECLARE @ResourceId int
DECLARE @NodeId int = 1 -- Community Catalogue
DECLARE @ResourceVersionId int
DECLARE @UserTimezoneOffset int = NULL

EXECUTE resources.[ResourceCreate] 
   @ResourceTypeId,
   @Title,
   @Description,
   @UserId,
   @UserTimezoneOffset,
   @ResourceVersionId OUTPUT

DECLARE @FileId int
INSERT INTO [resources].[File]([FileTypeId],[FileName],[FilePath],[FileSizeKb],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@FileTypeId, @FileName,@FilePath,1024,0,@UserId,SYSDATETIMEOFFSET(),@UserId,SYSDATETIMEOFFSET())
SELECT @FileId = SCOPE_IDENTITY()

INSERT INTO [resources].[GenericFileResourceVersion]
           ([ResourceVersionId],[FileId],[ScormAiccContent],[AuthoredYear],[AuthoredMonth],[AuthoredDayOfMonth]
           ,[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@ResourceVersionId, @FileId, 0,null, null, null,0,@UserId,SYSDATETIMEOFFSET(),@UserId,SYSDATETIMEOFFSET())

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO
--------------------------------------------------------
-- 2. Create a 'Published' Generic File Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 9 -- Generic File
DECLARE @Title nvarchar(128) ='Test PUBLISHED Generic File Resource'
DECLARE @Description nvarchar(512) ='Test PUBLISHED Generic File Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'
DECLARE @FileTypeId int = 1 -- doc
DECLARE @FileName nvarchar(128)='Logon wizard Error Messages.docx'
DECLARE @FilePath nvarchar(1024) = '04af408b-1c2b-4e21-ae68-5da9c29cc601'
DECLARE @AuthorName nvarchar(50) = 'Generic File Author'
DECLARE @ResourceId int
DECLARE @NodeId int = 1 -- Community Catalogue
DECLARE @ResourceVersionId int

EXECUTE resources.[ResourceCreate] 
   @ResourceTypeId,
   @Title,
   @Description,
   @UserId,
   @UserTimezoneOffset,
   @ResourceVersionId OUTPUT

DECLARE @FileId int
INSERT INTO [resources].[File]([FileTypeId],[FileName],[FilePath],[FileSizeKb],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@FileTypeId, @FileName,@FilePath,1024,0,@UserId,SYSDATETIMEOFFSET(),@UserId,SYSDATETIMEOFFSET())
SELECT @FileId = SCOPE_IDENTITY()

INSERT INTO [resources].[GenericFileResourceVersion]
           ([ResourceVersionId],[FileId],[ScormAiccContent],[AuthoredYear],[AuthoredMonth],[AuthoredDayOfMonth]
           ,[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@ResourceVersionId, @FileId,  0, null, null, null,0,@UserId,SYSDATETIMEOFFSET(),@UserId,SYSDATETIMEOFFSET())

DECLARE @MajorRevisionInd bit=1
DECLARE @Notes nvarchar(4000)='Initial Publication'
EXECUTE  [resources].[ResourceVersionPublish]  @ResourceVersionId,@MajorRevisionInd,@Notes,@UserId

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO