CREATE TABLE [resources].[RecentlyAdded] (
    [Id]           INT IDENTITY (1, 1) NOT NULL,
    [DisplayOrder] INT NOT NULL,
    [ResourceId]   INT NOT NULL,
    CONSTRAINT [PK_RecentlyAdded_1] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_resources.RecentlyAdded_Resource] FOREIGN KEY ([ResourceId]) REFERENCES [resources].[Resource] ([Id])
);

