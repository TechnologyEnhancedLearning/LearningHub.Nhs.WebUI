--------------------------------------------------------
-- 1. Create a 'Draft' Image Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 5 -- Image
DECLARE @Title nvarchar(128) ='Test DRAFT Image Resource'
DECLARE @Description nvarchar(512) ='Test DRAFT Image Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'
DECLARE @FileTypeId int = 29 -- png
DECLARE @FileName nvarchar(128)='community-cat.png'
DECLARE @FilePath nvarchar(1024) = '73799719-bb16-4ee1-b4fc-86eed3e940f8'
DECLARE @AltTag nvarchar(125)='Test image alt tag'
DECLARE @AuthorName nvarchar(50) = 'Image Author'
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

INSERT INTO [resources].[ImageResourceVersion] ([ResourceVersionId],[ImageFileId],[AltTag],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (@ResourceVersionId, @FileId, @AltTag, '', 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO

--------------------------------------------------------
-- 2. Create a 'Published' Image Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 5 -- Image
DECLARE @Title nvarchar(128) ='Test PUBLISHED Image Resource'
DECLARE @Description nvarchar(512) ='Test PUBLISHED Image Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'
DECLARE @FileTypeId int = 29 -- png
DECLARE @FileName nvarchar(128)='community-cat.png'
DECLARE @FilePath nvarchar(1024) = '73799719-bb16-4ee1-b4fc-86eed3e940f8'
DECLARE @AltTag nvarchar(125)='Test image alt tag'
DECLARE @AuthorName nvarchar(50) = 'Image Author'
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
     VALUES (@FileTypeId, @FileName,@FilePath,1024,0,@UserId,SYSDATETIMEOFFSET(),@UserId,SYSDATETIMEOFFSET())
SELECT @FileId = SCOPE_IDENTITY()

INSERT INTO [resources].[ImageResourceVersion] ([ResourceVersionId],[ImageFileId],[AltTag],[Description],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (@ResourceVersionId, @FileId, @AltTag, '', 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

DECLARE @MajorRevisionInd bit=1
DECLARE @Notes nvarchar(4000)='Initial Publication'
EXECUTE  [resources].[ResourceVersionPublish]  @ResourceVersionId,@MajorRevisionInd,@Notes,@UserId

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO