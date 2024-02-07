// <copyright file="NotificationTemplateRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System.Linq;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Enums;
    using LearningHub.Nhs.Repository.Interface;

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
            return this.GetAll().SingleOrDefault(x => x.Id == (int)templateId);
        }
    }
}
