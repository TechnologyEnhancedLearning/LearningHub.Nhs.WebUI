--------------------------------------------------------
-- 1. Create a 'Draft' Equipment Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 4 -- Equipment
DECLARE @Title nvarchar(128) ='Test DRAFT Equipment Resource'
DECLARE @Description nvarchar(512) ='Test DRAFT Equipment Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'

DECLARE @AddressId int
DECLARE @Address1 nvarchar(100)='Address1'
DECLARE @Address2 nvarchar(100)='Address2'
DECLARE @Address3 nvarchar(100)='Address3'
DECLARE @Address4 nvarchar(100)='Address4'
DECLARE @Town nvarchar(100)='Town'
DECLARE @County nvarchar(100)='County'
DECLARE @PostCode nvarchar(100)='PC1 1PC'


DECLARE @ContactName nvarchar(255)='Contact Name'
DECLARE @ContactTelephone nvarchar(255)='0123-123-1234'
DECLARE @ContactEmail nvarchar(255)='contact@email.co.uk'

DECLARE @AuthorName nvarchar(50) = 'Equipment Author'
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

INSERT INTO [hub].[Address] ([Address1],[Address2],[Address3],[Address4],[Town],[County],[PostCode],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
    VALUES (@Address1,@Address2,@Address3,@Address4,@Town,@County,@PostCode,0,@UserId,SYSDATETIMEOFFSET(),@UserId,SYSDATETIMEOFFSET())
SELECT @AddressId = SCOPE_IDENTITY()

INSERT INTO [resources].[EquipmentResourceVersion] ([ResourceVersionId],[ContactName],[ContactTelephone],[ContactEmail],[AddressId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (@ResourceVersionId, @ContactName, @ContactTelephone, @ContactEmail, @AddressId, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO

--------------------------------------------------------
-- 2. Create a 'Published' Equipment Resource
-- ** Specify fields
--------------------------------------------------------
BEGIN TRAN

DECLARE @UserId int =  368736
DECLARE @ResourceTypeId int = 4 -- Equipment
DECLARE @Title nvarchar(128) ='Test PUBLISHED Equipment Resource'
DECLARE @Description nvarchar(512) ='Test PUBLISHED Equipment Resource - Description. '
					+ 'Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nullam et malesuada ligula, non suscipit dui. Integer ullamcorper nulla facilisis, sodales mi quis, tempor lectus. Suspendisse potenti. Maecenas mollis aliquam ex, non accumsan lacus hendrerit non. Nunc id faucibus odio. Nam nec nisi odio. Duis sed magna non libero cursus vulputate eu eget purus. Curabitur finibus tellus non fringilla tristique. Ut dapibus, dui porta vestibulum rhoncus, dui risus blandit urna, quis faucibus massa arcu rhoncus ligula. Nulla facilisi.'

DECLARE @AddressId int
DECLARE @Address1 nvarchar(100)='Address1'
DECLARE @Address2 nvarchar(100)='Address2'
DECLARE @Address3 nvarchar(100)='Address3'
DECLARE @Address4 nvarchar(100)='Address4'
DECLARE @Town nvarchar(100)='Town'
DECLARE @County nvarchar(100)='County'
DECLARE @PostCode nvarchar(100)='PC1 1PC'


DECLARE @ContactName nvarchar(255)='Contact Name'
DECLARE @ContactTelephone nvarchar(255)='0123-123-1234'
DECLARE @ContactEmail nvarchar(255)='contact@email.co.uk'

DECLARE @AuthorName nvarchar(50) = 'Equipment Author'
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

INSERT INTO [hub].[Address] ([Address1],[Address2],[Address3],[Address4],[Town],[County],[PostCode],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
    VALUES (@Address1,@Address2,@Address3,@Address4,@Town,@County,@PostCode,0,@UserId,SYSDATETIMEOFFSET(),@UserId,SYSDATETIMEOFFSET())
SELECT @AddressId = SCOPE_IDENTITY()

INSERT INTO [resources].[EquipmentResourceVersion] ([ResourceVersionId],[ContactName],[ContactTelephone],[ContactEmail],[AddressId],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
	VALUES (@ResourceVersionId, @ContactName, @ContactTelephone, @ContactEmail, @AddressId, 0, @UserId, SYSDATETIMEOFFSET(), @UserId, SYSDATETIMEOFFSET())

DECLARE @MajorRevisionInd bit=1
DECLARE @Notes nvarchar(4000)='Initial Publication'
EXECUTE  [resources].[ResourceVersionPublish]  @ResourceVersionId,@MajorRevisionInd,@Notes,@UserId

INSERT INTO [resources].[ResourceVersionAuthor]
           ([ResourceVersionId],[AuthorUserId],[AuthorName],[Deleted],[CreateUserId],[CreateDate],[AmendUserId],[AmendDate])
     VALUES (@ResourceVersionId, NULL, @AuthorName, 0, @UserId, sysdatetimeoffset(), @UserId, sysdatetimeoffset())

COMMIT
GO