CREATE TABLE [resources].[ResourceAttribute] (
    [Id]           INT                IDENTITY (1, 1) NOT NULL,
    [ResourceId]   INT                NOT NULL,
    [AttributeId]  INT                NOT NULL,
    [Value]        NVARCHAR (255)     NOT NULL,
    [Deleted]      BIT                NOT NULL,
    [CreateUserId] INT                NOT NULL,
    [CreateDate]   DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]  INT                NOT NULL,
    [AmendDate]    DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Resources_ResourceAttribute] PRIMARY KEY CLUSTERED ([Id] ASC),
);


GO

ALTER TABLE [resources].[ResourceAttribute]  WITH CHECK ADD  CONSTRAINT [FK_ResourceAttribute_Attribute] FOREIGN KEY([AttributeId])
REFERENCES [hub].[Attribute] ([Id]);

GO

ALTER TABLE [resources].[ResourceAttribute]  WITH CHECK ADD  CONSTRAINT [FK_ResourceAttribute_Resource] FOREIGN KEY([ResourceId])
REFERENCES [resources].[Resource] ([Id]);

GO