CREATE TABLE [hierarchy].[CatalogueAccessRequest]
(
	[Id] INT IDENTITY (1, 1) NOT NULL,
	[UserId] INT NOT NULL,
	[CatalogueNodeId] INT NOT NULL,
    [EmailAddress] NVARCHAR(100) NOT NULL,
    [Message] NVARCHAR(MAX) NOT NULL,
    [ResponseMessage] NVARCHAR(MAX) NULL,
    [Status] INT NOT NULL DEFAULT 0,
    [CompletedDate] DATETIMEOFFSET NULL,
	[Deleted]           BIT                NOT NULL,
    [CreateUserId]      INT                NOT NULL,
    [CreateDate]        DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]       INT                NOT NULL,
    [AmendDate]         DATETIMEOFFSET (7) NOT NULL,
    CONSTRAINT [PK_Hierarchy_CatalogueAccessRequest] PRIMARY KEY CLUSTERED ([Id] ASC),
)
GO

ALTER TABLE [hierarchy].[CatalogueAccessRequest]  WITH CHECK ADD  CONSTRAINT [FK_CatalogueAccessRequest_CatalogueNode] FOREIGN KEY([CatalogueNodeId])
REFERENCES [hierarchy].[Node] ([Id])
GO

ALTER TABLE [hierarchy].[CatalogueAccessRequest] CHECK CONSTRAINT [FK_CatalogueAccessRequest_CatalogueNode]
GO

ALTER TABLE [hierarchy].[CatalogueAccessRequest]  WITH CHECK ADD  CONSTRAINT [FK_CatalogueAccessRequest_User] FOREIGN KEY([UserId])
REFERENCES [hub].[User] ([Id])
GO

ALTER TABLE [hierarchy].[CatalogueAccessRequest] CHECK CONSTRAINT [FK_CatalogueAccessRequest_User]
GO

--ALTER TABLE [hierarchy].[CatalogueAccessRequest]  WITH CHECK ADD  CONSTRAINT [FK_CatalogueAccessRequest_UserProfile] FOREIGN KEY([UserId])
--REFERENCES [external].[UserProfile] ([Id])
--GO

--ALTER TABLE [hierarchy].[CatalogueAccessRequest] CHECK CONSTRAINT [FK_CatalogueAccessRequest_UserProfile]
--GO