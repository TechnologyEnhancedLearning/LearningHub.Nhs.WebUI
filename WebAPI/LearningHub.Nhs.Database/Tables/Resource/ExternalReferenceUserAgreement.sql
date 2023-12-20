
CREATE TABLE [resources].[ExternalReferenceUserAgreement](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ExternalReferenceId] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[Deleted] [bit] NOT NULL,
	[CreateUserId] [int] NOT NULL,
	[CreateDate] [datetimeoffset](7) NOT NULL,
	[AmendUserId] [int] NOT NULL,
	[AmendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_ExternalReferenceUserAgreement] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [resources].[ExternalReferenceUserAgreement] ADD  CONSTRAINT [DF_ExternalReferenceUserAgreement_Deleted]  DEFAULT ((0)) FOR [Deleted]
GO

ALTER TABLE [resources].[ExternalReferenceUserAgreement]  WITH CHECK ADD  CONSTRAINT [FK_ExternalReferenceUserAgreement_ExternalReference] FOREIGN KEY([ExternalReferenceId])
REFERENCES [resources].[ExternalReference] ([Id])
GO

ALTER TABLE [resources].[ExternalReferenceUserAgreement] CHECK CONSTRAINT [FK_ExternalReferenceUserAgreement_ExternalReference]
GO