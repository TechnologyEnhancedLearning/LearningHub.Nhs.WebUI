--------------------------------------------------------
-- 1. Create a 'Draft' Audio Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 2 -- Audio
DECLARE @Title nvarchar(128) ='Test DRAFT Audio Resource'
DECLARE @Description nvarchar(512) ='Test DRAFT Audio Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'
DECLARE @FileTypeId int = 14 -- mp3
DECLARE @FileName nvarchar(128)='relaxation.mp3'
DECLARE @FilePath nvarchar(1024) = 'https://learninghubdevcontent-euno.streaming.media.azure.net/c66a1ed1-83d3-4020-862c-32c90d696606/SampleAudio1.ism/manifest'
DECLARE @AuthorName nvarchar(50) = 'Audio Author'
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

INSERT INTO [resources].[AudioResourceVersion] ([ResourceVersionId],[AudioFileId],[TranscriptFileId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (@ResourceVersionId, @FileId, null, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO

--------------------------------------------------------
-- 2. Create a 'Published' Audio Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 2 -- Audio
DECLARE @Title nvarchar(128) ='Test PUBLISHED Audio Resource'
DECLARE @Description nvarchar(512) ='Test PUBLISHED Audio Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'
DECLARE @FileTypeId int = 14 -- mp3
DECLARE @FileName nvarchar(128)='relaxation.mp3'
DECLARE @FilePath nvarchar(1024) = 'https://learninghubdevcontent-euno.streaming.media.azure.net/c66a1ed1-83d3-4020-862c-32c90d696606/SampleAudio1.ism/manifest'
DECLARE @AuthorName nvarchar(50) = 'Audio Author'
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

INSERT INTO [resources].[AudioResourceVersion] ([ResourceVersionId],[AudioFileId],[TranscriptFileId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (@ResourceVersionId, @FileId, null, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

DECLARE @MajorRevisionInd bit=1
DECLARE @Notes nvarchar(4000)='Initial Publication'
EXECUTE  [resources].[ResourceVersionPublish]  @ResourceVersionId,@MajorRevisionInd,@Notes,@UserId

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES
           (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO