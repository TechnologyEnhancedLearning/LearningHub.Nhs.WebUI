﻿namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Linq;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;

    /// <summary>
    /// The NotificationTemplateRepository class.
    /// </summary>
    public class NotificationTemplateRepository : GenericRepository<NotificationTemplate>, INotificationTemplateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationTemplateRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public NotificationTemplateRepository(LearningHubDbContext context, ITimezoneOffsetManager tzOffsetManager)
            : base(context, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetById.
        /// </summary>
        /// <param name="templateId">The templateId.</param>
        /// <returns>The notification template.</returns>
        public NotificationTemplate GetById(NotificationTemplates templateId)
        {
            return GetAll().SingleOrDefault(x => x.Id == (int)templateId);
        }
    }
}
