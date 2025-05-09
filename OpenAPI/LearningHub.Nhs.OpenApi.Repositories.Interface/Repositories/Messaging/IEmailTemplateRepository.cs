﻿namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Messaging
{
    using LearningHub.Nhs.Models.Entities.Messaging;

    /// <summary>
    /// The IEmailTemplateRepository.
    /// </summary>
    public interface IEmailTemplateRepository
    {
        /// <summary>
        /// The GetTemplate.
        /// </summary>
        /// <param name="id">The email template id.</param>
        /// <returns>The Email Template.</returns>
        EmailTemplate GetTemplate(int id);
    }
}
