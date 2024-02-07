// <copyright file="INotificationTemplateRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Interface
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;

    /// <summary>
    /// The INotificationTemplateRepository interface.
    /// </summary>
    public interface INotificationTemplateRepository : IGenericRepository<NotificationTemplate>
    {
        /// <summary>
        /// The GetById.
        /// </summary>
        /// <param name="templateId">The templateId.</param>
        /// <returns>The notification template.</returns>
        NotificationTemplate GetById(NotificationTemplates templateId);
    }
}
