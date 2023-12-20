/*
	Table changes for the LandingPage CMS updates
*/



IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Title'
          AND Object_ID = Object_ID(N'content.PageSectionDetail'))
BEGIN
    ALTER TABLE [content].[PageSectionDetail]
    DROP COLUMN Title
END
GO

IF (OBJECT_ID('content.FK_PageSectionDetail_AssettPosition', 'F') IS NOT NULL)
BEGIN
    ALTER TABLE [content].[PageSectionDetail]
    DROP CONSTRAINT FK_PageSectionDetail_AssettPosition
END
GO

IF EXISTS (SELECT 1 FROM sys.objects WHERE type = 'D' AND name = 'DF_PageSectionDetail_AssetPositionId')
BEGIN
    ALTER TABLE [content].[PageSectionDetail]
    DROP CONSTRAINT DF_PageSectionDetail_AssetPositionId
END
GO

IF EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'AssetPositionId'
          AND Object_ID = Object_ID(N'content.PageSectionDetail'))
BEGIN
    ALTER TABLE [content].[PageSectionDetail]
    DROP COLUMN AssetPositionId
END
GO

IF OBJECT_ID('content.AssetPosition', 'U') IS NOT NULL 
BEGIN
    DROP TABLE [content].[AssetPosition]
END
GO


-- Tidy up the Content data
IF EXISTS (	SELECT PageSectionId, COUNT(Id) AS number
	FROM [content].[PageSectionDetail]
	WHERE deleted = 0
	GROUP BY PageSectionId
	HAVING COUNT(Id) > 1)
BEGIN
    UPDATE [content].[PageSectionDetail]
    SET Deleted = 1
    WHERE PageSectionStatusId != 5

    UPDATE psd
    SET DELETED = 1
    FROM [content].[PageSectionDetail] psd
    INNER JOIN [content].[PageSection] ps ON psd.PageSectionId = ps.Id
    WHERE ps.Deleted = 1 and psd.Deleted = 0

    UPDATE psd
    SET DELETED = 1
    FROM [content].[PageSectionDetail] psd
    INNER JOIN (
	    SELECT PageSectionId, MAX(Id) AS MaxID
	    FROM [content].[PageSectionDetail]
	    WHERE deleted = 0
	    GROUP BY PageSectionId
    ) m ON psd.PageSectionId = m.PageSectionId
    WHERE m.MaxID != psd.Id
END
