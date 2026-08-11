DECLARE @CreateDate DATETIMEOFFSET(7) = SYSDATETIMEOFFSET();

SET IDENTITY_INSERT [hub].[ProfessionalBody] ON;

INSERT INTO [hub].[ProfessionalBody]
(
    [Id],
    [ProfessionalBody],
    [OrderByNumber],
    [PlaceholderText],
    [HelpText],
    [RegexPattern],
    [RegisterURL],
    [IsActive],
    [CreateDate]
)
SELECT
    Source.[Id],
    Source.[ProfessionalBody],
    Source.[OrderByNumber],
    Source.[PlaceholderText],
    Source.[HelpText],
    Source.[RegexPattern],
    Source.[RegisterURL],
    Source.[IsActive],
    @CreateDate
FROM
(
    VALUES
        (
            1,
            N'General Medical Council (GMC)',
            10,
            N'1234567',
            N'Enter your 7-digit GMC registration number.',
            N'^\d{7}$',
            N'https://www.gmc-uk.org/registration-and-licensing/our-registers',
            CAST(1 AS BIT)
        ),
        (
            2,
            N'Nursing and Midwifery Council (NMC)',
            20,
            N'12A3456E',
            N'Enter your NMC PIN exactly as shown on your registration.',
            N'^\d{2}[A-Z]\d{4}[A-Z]$',
            N'https://www.nmc.org.uk/registration/search-the-register/',
            CAST(1 AS BIT)
        ),
        (
            3,
            N'Health and Care Professions Council (HCPC)',
            30,
            N'PH12345',
            N'Enter your HCPC registration number, including the profession prefix if applicable.',
            N'^[A-Z]{2}\d{5,6}$',
            N'https://www.hcpc-uk.org/check-the-register/',
            CAST(1 AS BIT)
        ),
        (
            4,
            N'General Dental Council (GDC)',
            40,
            N'123456',
            N'Enter your GDC registration number exactly as issued.',
            NULL,
            N'https://www.gdc-uk.org/check-the-register',
            CAST(1 AS BIT)
        ),
        (
            5,
            N'General Pharmaceutical Council (GPhC)',
            50,
            N'2076543',
            N'Enter your GPhC registration number exactly as issued.',
            NULL,
            N'https://www.pharmacyregulation.org/registers',
            CAST(1 AS BIT)
        ),
        (
            6,
            N'General Chiropractic Council (GCC)',
            60,
            N'C1234',
            N'Enter your GCC registration number exactly as issued.',
            NULL,
            N'https://www.gcc-uk.org/search-the-register/',
            CAST(1 AS BIT)
        ),
        (
            7,
            N'General Osteopathic Council (GOsC)',
            70,
            N'1234',
            N'Enter your GOsC registration number exactly as issued.',
            NULL,
            N'https://register.osteopathy.org.uk/',
            CAST(1 AS BIT)
        ),
        (
            8,
            N'General Optical Council (GOC)',
            80,
            N'01-12345',
            N'Enter your GOC registration number exactly as issued.',
            NULL,
            N'https://optical.org/en/our_register/index.cfm',
            CAST(1 AS BIT)
        ),
        (
            9,
            N'Other (not listed)',
            999,
            N'ABC12345',
            N'Enter your professional registration number exactly as issued by your professional body.',
            NULL,
            NULL,
            CAST(1 AS BIT)
        )
) AS Source
(
    [Id],
    [ProfessionalBody],
    [OrderByNumber],
    [PlaceholderText],
    [HelpText],
    [RegexPattern],
    [RegisterURL],
    [IsActive]
)
WHERE NOT EXISTS
(
    SELECT 1
    FROM [hub].[ProfessionalBody] AS Existing
    WHERE Existing.[Id] = Source.[Id]
);

SET IDENTITY_INSERT [hub].[ProfessionalBody] OFF;
GO