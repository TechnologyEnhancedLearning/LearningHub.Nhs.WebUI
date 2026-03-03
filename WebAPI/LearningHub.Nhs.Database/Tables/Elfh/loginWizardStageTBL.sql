CREATE TABLE [elfh].[loginWizardStageTBL](
	[loginWizardStageId] [int] NOT NULL,
	[description] [nvarchar](128) NOT NULL,
	[reasonDisplayText] [nvarchar](1024) NOT NULL,
	[deleted] [bit] NOT NULL,
	[amendUserId] [int] NOT NULL,
	[amendDate] [datetimeoffset](7) NOT NULL,
 CONSTRAINT [PK_loginWizardStageTBL] PRIMARY KEY CLUSTERED 
(
	[loginWizardStageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
