-------------------------------------------------------------------------------
-- Author       Killian Davies
-- Created      01-01-2020
-- Purpose      Reassigns the ownership of the supplied resource version
--
-- Modification History
--
-- 27-05-2020  Killian Davies	Initial Revision
-- 24-12-2020  Dave Brown		Updated to affect the Amend dates as well as the Created dates
-- 11-01-2021  Dharmendra Verma	Card-6516, Set correct timezone information 
-------------------------------------------------------------------------------
CREATE PROCEDURE [resources].[ResourceVersionReassignOwnership]
(
	@ResourceVersionId int,
	@NewOwnerUserId int,
	@UserId int,
	@UserTimezoneOffset int = NULL
)

AS

BEGIN
	DECLARE @AmendDate datetimeoffset(7) = ISNULL(TODATETIMEOFFSET(DATEADD(mi, @UserTimezoneOffset, GETUTCDATE()), @UserTimezoneOffset), SYSDATETIMEOFFSET())

	DECLARE @ResourceTypeId int
	DECLARE @PublicationId int

	SELECT 
		@ResourceTypeId = r.ResourceTypeId,
		@PublicationId = PublicationId
	FROM	
		resources.ResourceVersion rv
	INNER JOIN	
		resources.[Resource] r ON rv.ResourceId = r.Id
	WHERE 
		rv.Id = @ResourceVersionId

	UPDATE	
		resources.[ResourceVersion]
	SET 
		CreateUserId=@NewOwnerUserId, 
		AmendUserId=@NewOwnerUserId, 
		AmendDate=@AmendDate
	WHERE 
		Id=@ResourceVersionId

	IF @ResourceTypeId = 1
	BEGIN

		UPDATE resources.ArticleResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0

	END
	IF @ResourceTypeId = 2 
	BEGIN
		UPDATE resources.AudioResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END
	IF @ResourceTypeId = 3 
	BEGIN
		UPDATE resources.EmbeddedResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END		
	IF @ResourceTypeId = 4 
	BEGIN
		UPDATE resources.EquipmentResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END
	IF @ResourceTypeId = 5 
	BEGIN
		UPDATE resources.ImageResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END
	IF @ResourceTypeId = 6 
	BEGIN
		UPDATE resources.ScormResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END
	IF @ResourceTypeId = 7 
	BEGIN
		UPDATE resources.VideoResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END
	IF @ResourceTypeId = 8 
	BEGIN
		UPDATE resources.WebLinkResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END
	IF @ResourceTypeId = 9 
	BEGIN
		UPDATE resources.GenericFileResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END
	IF @ResourceTypeId = 12 
	BEGIN
		UPDATE resources.HtmlResourceVersion
		SET 
			CreateUserId=@NewOwnerUserId, 
			AmendUserID=@NewOwnerUserId, 
			AmendDate=@AmendDate
		WHERE 
			ResourceVersionId=@ResourceVersionId
			AND Deleted=0
	END

	IF @PublicationId IS NOT NULL
	BEGIN

		UPDATE hierarchy.Publication
		SET CreateUserId = @NewOwnerUserId
		WHERE Id=@PublicationId AND Deleted=0

	END
	
	DECLARE @UserName nvarchar(50)
	DECLARE @ResourceVersionEventTypeId int = 5 -- Admin event type
	DECLARE @Details nvarchar(1024)
	SELECT @Details = 'Transferred resource ownership to UserName=' + UserName
	FROM hub.[User] WHERE Id=@NewOwnerUserId

	EXECUTE [resources].[ResourceVersionEventCreate]  @ResourceVersionId, @ResourceVersionEventTypeId, @Details, @UserId, @UserTimezoneOffset

END

GO
