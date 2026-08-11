DECLARE @CreateDate DATETIMEOFFSET(7) = SYSDATETIMEOFFSET();

SET IDENTITY_INSERT [hub].[JobRoleType] ON;

INSERT INTO [hub].[JobRoleType]
(
    [Id],
    [JobRoleType],
    [CreateDate]
)
SELECT
    Source.[Id],
    Source.[JobRoleType],
    @CreateDate
FROM
(
    VALUES
        (1,  N'Doctor / Dentist'),
        (2,  N'Nursing / Midwifery'),
        (3,  N'Allied Health Professional'),
        (4,  N'Healthcare Scientist'),
        (5,  N'Pharmacy'),
        (6,  N'Healthcare Support Worker'),
        (7,  N'Social Care Professional'),
        (8,  N'Social Care Support Worker'),
        (9,  N'Administrative / Clerical'),
        (10, N'Management / Leadership'),
        (11, N'Digital / Informatics / IT'),
        (12, N'Education / Training'),
        (13, N'Research'),
        (14, N'Estates / Facilities / Ancillary'),
        (15, N'Professional / Scientific / Technical'),
        (16, N'Volunteer / Carer / Family Support'),
        (17, N'Student'),
        (18, N'Other / Unknown')
) AS Source
(
    [Id],
    [JobRoleType]
)
WHERE NOT EXISTS
(
    SELECT 1
    FROM [hub].[JobRoleType] AS Existing
    WHERE Existing.[Id] = Source.[Id]
);

SET IDENTITY_INSERT [hub].[JobRoleType] OFF;
GO