CREATE TABLE [external].[ExternalSystem] (
    [Id]                  INT                IDENTITY (1, 1) NOT NULL,
    [Name]                NVARCHAR (100)     NOT NULL,
    [Code]                NVARCHAR (100)     NOT NULL,
    [CallbackUrl]         NVARCHAR (100)     NOT NULL,
    [SecretKey]           NVARCHAR (512)     NOT NULL,
    [TermsAndConditions]  NVARCHAR (MAX)     NOT NULL,
    [DefaultUserGroupId]  INT                NULL,
    [DefaultStaffGroupId] INT                NULL,
    [DefaultJobRoleId]    INT                NULL,
    [DefaultGradingId]    INT                NULL,
    [DefaultSpecialityId] INT                NULL,
    [DefaultLocationId]   INT                NULL,
    [Deleted]             BIT                DEFAULT ((0)) NOT NULL,
    [CreateUserId]        INT                NOT NULL,
    [CreateDate]          DATETIMEOFFSET (7) NOT NULL,
    [AmendUserId]         INT                NULL,
    [AmendDate]           DATETIMEOFFSET (7) NULL,
    CONSTRAINT [PK_ExternalSystem] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UC_Code] UNIQUE NONCLUSTERED ([Code] ASC)
);




GO
