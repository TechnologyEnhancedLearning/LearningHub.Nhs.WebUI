CREATE TABLE [resources].[ScormResourceReferenceEvent] (
    [Id]                                INT                IDENTITY (1, 1) NOT NULL,
    [ScormResourceReferenceEventTypeId] INT                NOT NULL,
    [ResourceReferenceId]               INT                NULL,
    [Url]                               NVARCHAR (1024)    NOT NULL,
    [HttpRefererer]                     NVARCHAR (1024)    NULL,
    [IPAddress]                         NVARCHAR (50)      NULL,
    [Deleted]                           BIT                NOT NULL,
    [CreateUserId]                      INT                NOT NULL,
    [CreateDate]                        DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]                       INT                NOT NULL,
    [AmendDate]                         DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_ScormResource_ReferenceEvent] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ScormResourceReferenceEvent_ResourceReference] FOREIGN KEY ([ResourceReferenceId]) REFERENCES [resources].[ResourceReference] ([Id]),
    CONSTRAINT [FK_ScormResourceReferenceEvent_ScormResourceReferenceEvent] FOREIGN KEY ([ScormResourceReferenceEventTypeId]) REFERENCES [resources].[ScormResourceReferenceEventType] ([Id])
);

