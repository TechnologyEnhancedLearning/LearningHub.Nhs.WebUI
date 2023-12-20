insert into [dbo].[emailTemplateTypeTbl]
(emailTemplateTypeId, emailTemplateTypeName, availableTags, deleted, amendUserId, amendDate)
values (22, 'User Contact', '[Subject][Body]', 0, 0, GETDATE());

insert into [dbo].[emailTemplateTbl] 
(emailTemplateId, emailTemplateTypeId, programmeComponentId, title, subject, body, deleted, amendUserID, amendDate, tenantId)
values (1803, 22, 11, 'User Contact', '[Subject]', '[Body]<br/><br/>The Learning Hub Support TeamHealth Education England<br/><br/>This e-mail and any files transmitted with it are confidential. If you are not the intended recipient, any reading, printing, storage, disclosure, copying or any other action taken in respect of this e-mail is prohibited and may be unlawful. If you are not the intended recipient, please notify the sender immediately by using the reply function and then permanently delete what you have received.', 0, 0, GETDATE(), 10);
