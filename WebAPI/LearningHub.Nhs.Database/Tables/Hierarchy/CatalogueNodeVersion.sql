CREATE TABLE [hierarchy].[CatalogueNodeVersion] (
    [Id]                INT                IDENTITY (1, 1) NOT NULL,
    [NodeVersionId]     INT                NOT NULL,
    [Name]              NVARCHAR (255)     NOT NULL,
    [Url]               NVARCHAR (1000)    NOT NULL,
    [BadgeUrl]          NVARCHAR (128)     NULL,
    [CardImageUrl]      NVARCHAR (128)     NULL,
    [BannerUrl]         NVARCHAR (128)     NULL,
    [CertificateUrl]    NVARCHAR (128)     NULL,
    [Description]       NVARCHAR (4000)    NOT NULL,
    [OwnerName]         NVARCHAR (250)     NULL,
    [OwnerEmailAddress] NVARCHAR (250)     NULL,
    [Notes]             NVARCHAR (MAX)     NULL,
    [Order]             INT                NOT NULL,
    [RestrictedAccess]  BIT                NOT NULL DEFAULT 0,
    [LastShownDate]     DATETIMEOFFSET (7) NULL,
    [Deleted]           BIT                NOT NULL,
    [CreateUserId]      INT                NOT NULL,
    [CreateDate]        DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]       INT                NOT NULL,
    [AmendDate]         DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Hierarchy_CatalogueNodeVersion] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CatalogueNodeVersion_NodeVersion] FOREIGN KEY ([NodeVersionId]) REFERENCES [hierarchy].[NodeVersion] ([Id])
);


GO


GO


GO


GO


GO
