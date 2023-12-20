  UPDATE [dbo].[emailTemplateTBL]
  SET  
  [body] = cast(replace(cast([body] as nvarchar(max)),'Health Education England', 'NHS England') as ntext),
  [amendUserID] = 4,
  [amendDate] = SYSDATETIMEOFFSET()
  WHERE [body] LIKE '%Health Education England%'

  UPDATE
  [dbo].[emailTemplateTBL]
  SET 
  [body] =  cast(replace(cast([body] as nvarchar(max)),'HEE ', 'NHSE ') as ntext),
  [amendUserID] = 4,
  [amendDate] = SYSDATETIMEOFFSET()
  WHERE [body] LIKE '%HEE %'

  UPDATE
  [dbo].[emailTemplateTBL]
  SET 
  [body] =  cast(replace(cast([body] as nvarchar(max)),'(HEE)', '(NHSE)') as ntext),
  [amendUserID] = 4,
  [amendDate] = SYSDATETIMEOFFSET()
  WHERE [body] LIKE '%(HEE)%'

  UPDATE
  [dbo].[termsAndConditionsTBL]
  SET 
  [details] = cast(replace(cast([details] as nvarchar(max)),'Health Education England', 'NHS England') as ntext),
  [amendUserID] = 4,
  [amendDate] = SYSDATETIMEOFFSET()
  WHERE [details] LIKE '%Health Education England%'

  UPDATE
  [dbo].[termsAndConditionsTBL]
  SET [details] =  cast(replace(cast([details] as nvarchar(max)),'HEE ', 'NHSE ') as ntext),
  [amendUserID] = 4,
  [amendDate] = SYSDATETIMEOFFSET()
  WHERE [details] LIKE '%HEE %'

  UPDATE
  [dbo].[termsAndConditionsTBL]
  SET [details] =  cast(replace(cast([details] as nvarchar(max)),'(HEE)', '(NHSE)') as ntext),
  [amendUserID] = 4,
  [amendDate] = SYSDATETIMEOFFSET()
  WHERE [details] LIKE '%(HEE)%'
