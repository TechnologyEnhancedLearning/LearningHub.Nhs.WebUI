-- Define the date 6 months ago
DECLARE @SixMonthsAgo DATE = DATEADD(MONTH, -6, GETDATE());


IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LogArchive' AND schema_id = SCHEMA_ID('hub'))
BEGIN
    SELECT TOP (0) *
    INTO hub.LogArchive
    FROM hub.Log;
END


INSERT INTO hub.LogArchive ([Application], [Logged], [Level], [Message], [Logger], [Callsite], [Exception], [UserId], [Username])
SELECT [Application], [Logged], [Level], [Message], [Logger], [Callsite], [Exception], [UserId], [Username]
FROM hub.Log
WHERE [Logged] < @SixMonthsAgo;


DELETE FROM hub.Log WHERE [Logged] < @SixMonthsAgo;


