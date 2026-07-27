
--------------------------------------------------------------------------
--	Jignesh Jethwani 02-03-2018	Initial version
--  Jignesh Jethwani 02-10-2023	TD-2913, performance improvement, added no lock to select statement
--  Tobi Awe 24-09-2025		Updated table schema
--------------------------------------------------------------------------
CREATE PROCEDURE [elfh].[proc_UserHistoryAttributeSave]
(
	@userHistoryAttributeId int OUTPUT,
	@userHistoryId int,
	@attributeId int,
	@intValue int,
	@textValue nvarchar(255),
	@booleanValue bit,
	@dateValue datetimeoffset,
	@deleted bit,
	@amendUserId int,
	@amendDate datetimeoffset
)
AS
BEGIN
	DECLARE @currentIntValue int
	DECLARE @currentTextValue nvarchar(255)
	DECLARE @currentBooleanValue bit
	DECLARE @currentDateValue datetimeoffset
	SELECT 
		@userHistoryAttributeId = userHistoryAttributeId,
		@currentIntValue = intValue,
		@currentTextValue = textValue,
		@currentBooleanValue = booleanValue,
		@currentDateValue = dateValue
	FROM
		elfh.userHistoryAttributeTBL WITH (NOLOCK)
	WHERE
		userHistoryId = @userHistoryId
	AND 
		attributeId = @attributeId
	IF @userHistoryAttributeId IS NULL 
	BEGIN
		IF @intValue IS NOT NULL
			OR ISNULL(@textValue, '') != ''
			OR @booleanValue IS NOT NULL
			OR @dateValue IS NOT NULL
		BEGIN
			INSERT INTO elfh.userHistoryAttributeTBL(userHistoryId,
												  attributeId,
												  intValue,
												  textValue,
												  booleanValue,
												  dateValue,
												  deleted,
												  amendUserId,
												  amendDate)
			SELECT 
				userHistoryId = @userHistoryId,
				attributeId = @attributeId,
				intValue = @intValue,
				textValue = @textValue,
				booleanValue = @booleanValue,
				dateValue = @dateValue,
				deleted = @deleted,
				amendUserId = @amendUserId,
				@amendDate
			SELECT @userHistoryAttributeId = SCOPE_IDENTITY()
		END
	END
	ELSE
	BEGIN
		-- Only update when an Attribute Value has changed
		IF (@intValue != @currentIntValue
			OR ISNULL(@textValue, '') != ISNULL(@currentTextValue,'')
			OR @booleanValue != @currentBooleanValue
			OR @dateValue != @currentDateValue)
		BEGIN
			UPDATE 
				elfh.userHistoryAttributeTBL
			SET 
				intValue = @intValue,
				textValue = @textValue,
				booleanValue = @booleanValue,
				dateValue = @dateValue,
				deleted = @deleted,
				amendUserId = @amendUserId,
				amendDate = @amendDate
			WHERE
				userHistoryAttributeId = @userHistoryAttributeId
		END
	END
END
