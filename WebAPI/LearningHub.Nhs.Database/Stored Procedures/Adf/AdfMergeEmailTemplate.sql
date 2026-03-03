-------------------------------------------------------------------------------
-- Author       Sarathlal
-- Created      04-11-2025
-- Purpose      ELFH-LH Data sync 
--
-- Modification History
--
-- 04-11-2025  Sarathlal	    Initial Revision
-------------------------------------------------------------------------------
CREATE PROCEDURE [dbo].[AdfMergeEmailTemplate]
    @EmailTemplateList [dbo].[EmailTemplate] READONLY
AS
BEGIN
    SET NOCOUNT ON;
	SET IDENTITY_INSERT [elfh].[emailTemplateTBL] ON;
    MERGE [elfh].[emailTemplateTBL] AS TARGET
    USING @EmailTemplateList AS SOURCE
        ON TARGET.[emailTemplateId] = SOURCE.[emailTemplateId]

    WHEN MATCHED THEN
        UPDATE SET 
            TARGET.[emailTemplateTypeId]   = SOURCE.[emailTemplateTypeId],
            TARGET.[programmeComponentId]  = SOURCE.[programmeComponentId],
            TARGET.[title]                 = SOURCE.[title],
            TARGET.[subject]               = SOURCE.[subject],
            TARGET.[body]                  = SOURCE.[body],
            TARGET.[deleted]               = SOURCE.[deleted],
            TARGET.[amendUserID]           = SOURCE.[amendUserID],
            TARGET.[amendDate]             = SOURCE.[amendDate],
            TARGET.[tenantId]              = SOURCE.[tenantId]

    WHEN NOT MATCHED BY TARGET THEN
        INSERT (
            [emailTemplateId],
            [emailTemplateTypeId],
            [programmeComponentId],
            [title],
            [subject],
            [body],
            [deleted],
            [amendUserID],
            [amendDate],
            [tenantId]
        )
        VALUES (
            SOURCE.[emailTemplateId],
            SOURCE.[emailTemplateTypeId],
            SOURCE.[programmeComponentId],
            SOURCE.[title],
            SOURCE.[subject],
            SOURCE.[body],
            SOURCE.[deleted],
            SOURCE.[amendUserID],
            SOURCE.[amendDate],
            SOURCE.[tenantId]
        );
	SET IDENTITY_INSERT [elfh].[emailTemplateTBL] OFF;
END;
GO
