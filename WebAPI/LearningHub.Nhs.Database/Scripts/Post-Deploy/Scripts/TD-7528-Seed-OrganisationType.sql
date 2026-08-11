DECLARE @CreateDate DATETIMEOFFSET(7) = SYSDATETIMEOFFSET();

SET IDENTITY_INSERT [hub].[OrganisationType] ON;

INSERT INTO [hub].[OrganisationType]
(
    [Id],
    [OrganisationType],
    [Description],
    [EligibilityLevelId],
    [CreateDate]
)
SELECT
    Source.[Id],
    Source.[OrganisationType],
    Source.[Description],
    Source.[EligibilityLevelId],
    @CreateDate
FROM
(
    VALUES
        (1,  N'NHS Provider', N'Acute, Mental Health, Community, Ambulance, Specialist Trusts', 3),
        (2,  N'NHS Primary Care Provider', N'GP Practices, PCNs, Federations, Community Pharmacies, Dental, Optical', 3),
        (3,  N'NHS Regional Body', N'ICB, ICS, ICP, CSU', 3),
        (4,  N'NHS Arm''s Length Body', N'NHSBSA, NHSBT, NHS Resolution etc.', 3),
        (5,  N'Public Health Body', N'UKHSA and approved government health bodies', 3),
        (6,  N'Local Government', N'Local authorities delivering health/care functions', 3),
        (7,  N'Emergency Service', N'Ambulance, Fire, Police', 3),
        (8,  N'Defence Medical Service', N'RAF, Army, Navy medical services', 3),
        (9,  N'Regulatory Body', N'CQC, MHRA, HTA', 3),
        (10, N'UK National Health Body', N'NHS Scotland, HEIW, NIMDTA (subject to agreements)', 3),

        (11, N'Charity', N'Subject to assessment', 2),
        (12, N'Community Interest Company (CIC)', N'Subject to assessment', 2),
        (13, N'Voluntary Organisation', N'Subject to assessment', 2),
        (14, N'Academic Partnership', N'Subject to assessment', 2),
        (15, N'Professional Body / Royal College', N'Subject to assessment', 2),
        (16, N'University / Academic Organisation', N'Subject to assessment', 2),
        (17, N'Innovation Network', N'Health Innovation Networks etc.', 2),
        (18, N'Cross-sector Partnership', N'NHS-led partnerships', 2),
        (19, N'Independent Care Provider (NHS Commissioned)', N'NHS commissioned services', 2),
        (20, N'Crown Dependency', N'Jersey, Guernsey, Isle of Man', 2),
        (21, N'British Overseas Territory', N'Gibraltar etc.', 2),
        (22, N'Advisory or Research Body', N'NICE, NIHR, King''s Fund, Health Foundation', 2),
        (23, N'Other Health & Care Organisation', N'Catch-all requiring governance review', 2),

        (24, N'Independent Care Provider (Private)', N'Commercial route only', 1),
        (25, N'Commercial Training Provider', N'Commercial route only', 1),
        (26, N'Commercial Organisation', N'Commercial route only', 1),
        (27, N'Consultancy', N'Commercial route only', 1),
        (28, N'Commercial Healthcare Provider', N'Commercial route only', 1),
        (29, N'Staffing Agency', N'Commercial route only', 1),
        (30, N'Informal Collaborative', N'No legal entity', 1),
        (31, N'General Membership Organisation', N'Outside scope', 1),
        (32, N'Organisation Failing Governance Requirements', N'Security/IG risk', 1),
        (33, N'Organisation Outside Health & Care', N'Outside policy scope', 1)
) AS Source
(
    [Id],
    [OrganisationType],
    [Description],
    [EligibilityLevelId]
)
WHERE NOT EXISTS
(
    SELECT 1
    FROM [hub].[OrganisationType] AS Existing
    WHERE Existing.[Id] = Source.[Id]
);

SET IDENTITY_INSERT [hub].[OrganisationType] OFF;
GO