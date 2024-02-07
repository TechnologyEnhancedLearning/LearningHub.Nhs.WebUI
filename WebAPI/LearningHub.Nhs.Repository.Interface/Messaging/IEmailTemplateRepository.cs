// <copyright file="IEmailTemplateRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface.Messaging
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
