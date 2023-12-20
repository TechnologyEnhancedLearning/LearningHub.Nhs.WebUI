CREATE TABLE [resources].[ScormResourceVersionManifest](
    [Id]                     INT                IDENTITY (1, 1) NOT NULL,
    [ScormResourceVersionId] INT                NOT NULL,
    [Title]                  NVARCHAR (255)     NULL,
    [Author]                 NVARCHAR (512)     NULL,
    [Duration]               NVARCHAR (128)     NULL,
    [Description]            NVARCHAR (MAX)     NULL,
    [Keywords]               NVARCHAR (1000)    NULL,
    [ManifestURL]            NVARCHAR (255)     NULL,
    [QuicklinkId]            NVARCHAR (30)      NULL,
    [CatalogEntry]           NVARCHAR (128)     NULL,
    [MasteryScore]           DECIMAL (16, 2)    NULL,
    [Copyright]              NVARCHAR (30)      NULL,
    [ResourceIdentifier]     NVARCHAR (128)     NULL,
    [ItemIdentifier]         NVARCHAR (128)     NULL,
    [TemplateVersion]        NVARCHAR (50)      NULL,
    [Deleted]                BIT                NOT NULL,
    [CreateUserId]           INT                NOT NULL,
    [CreateDate]             DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]            INT                NOT NULL,
    [AmendDate]              DATETIMEOFFSET (7) NOT NULL,
    [LaunchData]             NVARCHAR (MAX)     NULL,
    [MaxTimeAllowed]         NVARCHAR (13)      NULL,
    [TimeLimitAction]        NVARCHAR (20)      NULL,
 CONSTRAINT [PK_ResourceVersion_ScormResourceVersionManifest] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)
)
GO

ALTER TABLE [resources].[ScormResourceVersionManifest]  ADD  CONSTRAINT [FK_ScormResourceVersionManifest_AmendUser] FOREIGN KEY([AmendUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ScormResourceVersionManifest] ADD  CONSTRAINT [FK_ScormResourceVersionManifest_CreateUser] FOREIGN KEY([CreateUserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [resources].[ScormResourceVersionManifest]  ADD  CONSTRAINT [FK_ScormResourceVersionManifest_ScormResourceVersion] FOREIGN KEY([ScormResourceVersionId])
REFERENCES [resources].[ScormResourceVersion] ([Id])
GO
