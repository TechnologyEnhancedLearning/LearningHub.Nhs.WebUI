--------------------------------------------------------
-- 1. Create a 'Draft' Embedded Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 3 -- Embedded
DECLARE @Title nvarchar(128) ='Test DRAFT Embedded Resource'
DECLARE @Description nvarchar(512) ='Test DRAFT Embedded Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'
DECLARE @EmbedCode nvarchar(255)='EmbedCode1234'
DECLARE @AuthorName nvarchar(50) = 'Embedded Author'
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

INSERT INTO [resources].[EmbeddedResourceVersion] ([ResourceVersionId],[EmbedCode],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (@ResourceVersionId, @EmbedCode, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO

--------------------------------------------------------
-- 2. Create a 'Published' Embedded Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 3 -- Embedded
DECLARE @Title nvarchar(128) ='Test PUBLISHED Embedded Resource'
DECLARE @Description nvarchar(512) ='Test PUBLISHED Embedded Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'
DECLARE @EmbedCode nvarchar(255)='EmbedCode1234'
DECLARE @AuthorName nvarchar(50) = 'Embedded Author'
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

	
INSERT INTO [resources].[EmbeddedResourceVersion] ([ResourceVersionId],[EmbedCode],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (@ResourceVersionId, @EmbedCode, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

DECLARE @MajorRevisionInd bit=1
DECLARE @Notes nvarchar(4000)='Initial Publication'
EXECUTE  [resources].[ResourceVersionPublish]  @ResourceVersionId,@MajorRevisionInd,@Notes,@UserId

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO
