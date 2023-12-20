CREATE TABLE [resources].[ResourceVersionRatingSummary] (
    [Id]                INT                IDENTITY (1, 1) NOT NULL,
    [ResourceVersionId] INT                NOT NULL,
    [AverageRating]     DECIMAL (2, 1)     CONSTRAINT [DF_ResourceVersionRatingSummary_AverageRating] DEFAULT ((0)) NOT NULL,
    [RatingCount]       INT                CONSTRAINT [DF_ResourceVersionRatingSummary_RatingCount] DEFAULT ((0)) NOT NULL,
    [Rating1StarCount]  INT                CONSTRAINT [DF_ResourceVersionRatingSummary_Rating1StarCount] DEFAULT ((0)) NOT NULL,
    [Rating2StarCount]  INT                CONSTRAINT [DF_ResourceVersionRatingSummary_Rating2StarCount] DEFAULT ((0)) NOT NULL,
    [Rating3StarCount]  INT                CONSTRAINT [DF_ResourceVersionRatingSummary_Rating3StarCount] DEFAULT ((0)) NOT NULL,
    [Rating4StarCount]  INT                CONSTRAINT [DF_ResourceVersionRatingSummary_Rating4StarCount] DEFAULT ((0)) NOT NULL,
    [Rating5StarCount]  INT                CONSTRAINT [DF_ResourceVersionRatingSummary_Rating5StarCount] DEFAULT ((0)) NOT NULL,
    [Deleted]           BIT                NOT NULL,
    [CreateUserId]      INT                NOT NULL,
    [CreateDate]        DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]       INT                NOT NULL,
    [AmendDate]         DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_ResourceVersionRatingSummary] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ResourceVersionRatingSummary_ResourceVersion] FOREIGN KEY ([ResourceVersionId]) REFERENCES [resources].[ResourceVersion] ([Id])
);

