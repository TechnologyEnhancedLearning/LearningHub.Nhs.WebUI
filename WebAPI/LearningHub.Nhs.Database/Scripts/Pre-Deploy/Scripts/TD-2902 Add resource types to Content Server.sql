

IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'resources' 
                  AND TABLE_NAME = 'ResourceReferenceEventType'))
BEGIN
    CREATE TABLE [resources].[ResourceReferenceEventType] (
        [Id]           INT                IDENTITY (1, 1) NOT NULL,
        [Name]         NVARCHAR (32)      NULL,
        [Description]  NVARCHAR (200)     NULL,
        [Deleted]      BIT                NOT NULL,
        [CreateUserId] INT                NOT NULL,
        [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
        [AmendUserId]  INT                NOT NULL,
        [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
        CONSTRAINT [PK_ResourceReferenceEventType] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    SET IDENTITY_INSERT [resources].[ResourceReferenceEventType] ON

    INSERT INTO [resources].[ResourceReferenceEventType] (Id, [Name], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    SELECT Id, [Name], [Description], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate
    FROM    [resources].[ScormResourceReferenceEventType]

    SET IDENTITY_INSERT [resources].[ResourceReferenceEventType] OFF
END


IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
                WHERE TABLE_SCHEMA = 'resources'
                  AND TABLE_NAME = 'ResourceReferenceEvent'))
BEGIN

    CREATE TABLE [resources].[ResourceReferenceEvent] (
        [Id]                           INT                IDENTITY (1, 1) NOT NULL,
        [ResourceReferenceEventTypeId] INT                NOT NULL,
        [ResourceReferenceId]          INT                NULL,
        [Url]                          NVARCHAR (1024)    NOT NULL,
        [HttpRefererer]                NVARCHAR (1024)    NULL,
        [IPAddress]                    NVARCHAR (50)      NULL,
        [Deleted]                      BIT                NOT NULL,
        [CreateUserId]                 INT                NOT NULL,
        [CreateDate]                   DATETIMEOFFSET (7) NOT NULL,
        [AmendUserId]                  INT                NOT NULL,
        [AmendDate]                    DATETIMEOFFSET (7) NOT NULL,
        CONSTRAINT [PK_ResourceReferenceEvent] PRIMARY KEY CLUSTERED ([Id] ASC),
        CONSTRAINT [FK_ResourceReferenceEvent_ResourceReference] FOREIGN KEY ([ResourceReferenceId]) REFERENCES [resources].[ResourceReference] ([Id]),
        CONSTRAINT [FK_ResourceReferenceEvent_ResourceReferenceEventType] FOREIGN KEY ([ResourceReferenceEventTypeId]) REFERENCES [resources].[ResourceReferenceEventType] ([Id])
    );


    SET IDENTITY_INSERT [resources].[ResourceReferenceEvent] ON

    INSERT INTO [resources].[ResourceReferenceEvent] (Id, [ResourceReferenceEventTypeId], [ResourceReferenceId], [Url], [HttpRefererer], [IPAddress], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate)
    SELECT Id, [ScormResourceReferenceEventTypeId], [ResourceReferenceId], [Url], [HttpRefererer], [IPAddress], Deleted, CreateUserId, CreateDate, AmendUserId, AmendDate
    FROM    [resources].[ScormResourceReferenceEvent]

    SET IDENTITY_INSERT [resources].[ResourceReferenceEvent] OFF
END

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = 'resources'
              AND TABLE_NAME = 'ScormResourceReferenceEvent'))
BEGIN
    DROP TABLE [resources].[ScormResourceReferenceEvent]
END

IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES
            WHERE TABLE_SCHEMA = 'resources'
              AND TABLE_NAME = 'ScormResourceReferenceEventType'))
BEGIN
    DROP TABLE [resources].[ScormResourceReferenceEventType]
END


IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'GetScormContentServerDetailsForLHExternalReference')
BEGIN
    DROP PROCEDURE resources.GetScormContentServerDetailsForLHExternalReference
END
GO
