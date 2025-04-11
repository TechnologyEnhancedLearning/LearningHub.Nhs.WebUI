namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Messaging
{
    using System.Linq;
    using LearningHub.Nhs.Models.Entities.Messaging;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Messaging;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The EmailTemplateRepository.
    /// </summary>
    public class EmailTemplateRepository : GenericRepository<EmailTemplate>, IEmailTemplateRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailTemplateRepository"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public EmailTemplateRepository(LearningHubDbContext context, ITimezoneOffsetManager tzOffsetManager)
            : base(context, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetTemplate.
        /// </summary>
        /// <param name="id">The email template id.</param>
        /// <returns>The Email Template.</returns>
        public EmailTemplate GetTemplate(int id)
        {
            return DbContext.EmailTemplate.AsNoTracking()
                .Include(x => x.EmailTemplateLayout)
                .SingleOrDefault(et => et.Id == id);
        }
    }
}
