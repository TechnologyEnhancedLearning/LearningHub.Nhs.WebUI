  INSERT INTO MoodleInstanceConfigs 
  (ShortName, BaseUrl, TokenSecretName, EnabledEndpoints, Weighting, IsEnabled, CreatedAt, UpdatedAt)
VALUES 
  ('moodle-prod', 'https://learn.learninghub.nhs.uk/', 'LearningHubMoodleClientSecretProd', 'users,courses,grades', 100, 1, GETUTCDATE(), GETUTCDATE());