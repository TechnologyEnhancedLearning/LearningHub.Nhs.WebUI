INSERT INTO [hub].[AttributeType]
(
    [Id],
    [Name],
    [Deleted],
    [CreateUserId],
    [CreateDate],
    [AmendUserId],
    [AmendDate]
)
VALUES
(   5,
    'External Reference',
    0,
    4,
    '2026-11-05 15:00:26.7506886 +00:00',
    4,
    '2026-11-05 15:00:26.7506886 +00:00'
);


INSERT INTO [hub].[Attribute]
(
    [AttributeTypeId],
    [Name],
    [Description],
    [Deleted],
    [CreateUserId],
    [CreateDate],
    [AmendUserId],
    [AmendDate]
)
VALUES
(
    5,
    'OpenAthens_UserId',
    NULL,
    0,
    4,
    '2026-11-05 15:00:26.7506886 +00:00',
    4,
    '2026-11-05 15:00:26.7506886 +00:00'
),
(
    5,
    'OpenAthens_Idp',
    NULL,
    0,
    4,
    '2026-11-05 15:00:26.7506886 +00:00',
    4,
    '2026-11-05 15:00:26.7506886 +00:00'
);
 