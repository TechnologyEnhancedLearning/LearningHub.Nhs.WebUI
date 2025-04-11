namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories
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
