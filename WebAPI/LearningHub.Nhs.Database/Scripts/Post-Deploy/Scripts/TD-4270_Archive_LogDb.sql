

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LogArchive' AND schema_id = SCHEMA_ID('hub'))
BEGIN
    SELECT TOP (0) *
    INTO hub.LogArchive
    FROM hub.Log;
END
GO

DECLARE @SixMonthsAgo DATE;
SET @SixMonthsAgo = DATEADD(MONTH, -6, GETDATE());
INSERT INTO hub.LogArchive ([Application], [Logged], [Level], [Message], [Logger], [Callsite], [Exception], [UserId], [Username])
SELECT [Application], [Logged], [Level], [Message], [Logger], [Callsite], [Exception], [UserId], [Username]
FROM hub.Log
WHERE [Logged] < @SixMonthsAgo;
GO

DECLARE @SixMonthsAgo DATE;
SET @SixMonthsAgo = DATEADD(MONTH, -6, GETDATE());
DELETE FROM hub.Log WHERE [Logged] < @SixMonthsAgo;
GO