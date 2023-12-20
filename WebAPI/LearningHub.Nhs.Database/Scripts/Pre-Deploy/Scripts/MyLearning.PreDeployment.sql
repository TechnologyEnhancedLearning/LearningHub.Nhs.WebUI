/* 6909 - 22/10/2020 Rob S - Add new ClientDateTime column to activity.MediaResourceActivityInteraction table.
--------------------------------------------------------------------------------------
    RobS - 22 Oct 2020
	Card 6909 - Add ClientDateTime column to MediaResourceActivityInteraction table.
                Fill old records with existing CreateDate column value.
--------------------------------------------------------------------------------------*/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'ClientDateTime'
          AND Object_ID = Object_ID(N'activity.MediaResourceActivityInteraction'))
BEGIN
    
    ALTER TABLE [activity].[MediaResourceActivityInteraction]
    ADD ClientDateTime DateTimeOffset(7) NULL;

	DECLARE @SQL NVARCHAR(1000)
    SELECT @SQL = N'UPDATE [activity].[MediaResourceActivityInteraction] SET ClientDateTime = CreateDate'
    EXEC sp_executesql @SQL

    ALTER TABLE [activity].[MediaResourceActivityInteraction]
    ALTER COLUMN ClientDateTime DateTimeOffset(7) NOT NULL

END


/* 6907 - 22/10/2020 Rob S - Change VideoResourceVersion.DuratonInMinutes to DurationInMilliseconds, allowing duration to be more accurate. Same for audio.
--------------------------------------------------------------------------------------
    RobS - 22 Oct 2020
	Card 6909 - Change VideoResourceVersion.DuratonInMinutes from integer to timespan, 
                allowing duration to be more accurate, down to seconds or milliseconds. 
                Same for AudioResourceVersion.DuratonInMinutes.
--------------------------------------------------------------------------------------*/

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'DurationInMilliseconds'
          AND Object_ID = Object_ID(N'resources.VideoResourceVersion'))
BEGIN

	EXEC sp_rename 'resources.VideoResourceVersion.DurationInMinutes', 'DurationInMilliseconds', 'COLUMN';

	-- Convert existing values from minutes to milliseconds. On dev, test and prod a separate script will update these to be accurate.
    EXEC sp_executesql N'UPDATE [resources].[VideoResourceVersion] SET DurationInMilliseconds = DurationInMilliseconds * 60 * 1000 WHERE DurationInMilliseconds IS NOT NULL'; 

END
GO

IF NOT EXISTS(SELECT 1 FROM sys.columns 
          WHERE Name = N'Duration'
          AND Object_ID = Object_ID(N'resources.AudioResourceVersion'))
BEGIN

	EXEC sp_rename 'resources.AudioResourceVersion.DurationInMinutes', 'DurationInMilliseconds', 'COLUMN';

	-- Convert existing values from minutes to milliseconds. On dev, test and prod a separate script will update these to be accurate.
    EXEC sp_executesql N'UPDATE [resources].[AudioResourceVersion] SET DurationInMilliseconds = DurationInMilliseconds * 60 * 1000 WHERE DurationInMilliseconds IS NOT NULL'; 

END
GO


/* 7178 - 18/01/2021 Rob S - Backfill the NodePathId on all ResourceActivity records.
--------------------------------------------------------------------------------------
    RobS - 18 Jan 2021
	Card 8257 - Backfill the NodePathId on all ResourceActivity records. Needed by 
                My Learning Screen, to show resource reference Id in report.
--------------------------------------------------------------------------------------*/

IF NOT EXISTS(SELECT 1 FROM [activity].[ResourceActivity] WHERE NodePathId IS NOT NULL)
BEGIN
    
    -- Get NodePathId from ResourceReference.
    UPDATE ra
    SET ra.NodePathId = rr.NodePathId
    FROM [activity].[ResourceActivity] ra
    LEFT JOIN [resources].[ResourceReference] rr ON ra.ResourceId = rr.ResourceId

    -- Make column non-nullable.
    ALTER TABLE [activity].[ResourceActivity]
    ALTER COLUMN NodePathId INT NOT NULL

END

GO

