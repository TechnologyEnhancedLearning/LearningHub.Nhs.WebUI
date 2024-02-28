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


