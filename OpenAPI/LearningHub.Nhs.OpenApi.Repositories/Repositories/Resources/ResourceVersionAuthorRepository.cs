namespace LearningHub.Nhs.OpenApi.Repositories.Repositories.Resources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The resource version author repository.
    /// </summary>
    public class ResourceVersionAuthorRepository : GenericRepository<ResourceVersionAuthor>, IResourceVersionAuthorRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceVersionAuthorRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ResourceVersionAuthorRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="resourceVersionAuthorId">The resource version author id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task DeleteAsync(int userId, int resourceVersionAuthorId)
        {
            var amendDate = DateTimeOffset.Now;

            var resourceVersionAuthor = DbContext.ResourceVersionAuthor
                    .Where(r => r.Id == resourceVersionAuthorId)
                    .SingleOrDefault();

            if (resourceVersionAuthor != null)
            {
                SetAuditFieldsForDelete(userId, resourceVersionAuthor);

                await DbContext.SaveChangesAsync();
            }
        }

        /// <summary>
        /// The delete async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <param name="authorId">The resource version author id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ResourceVersionAuthor> GetByResourceVersionAndAuthorAsync(int resourceVersionId, int authorId)
        {
            return await DbContext.ResourceVersionAuthor.AsNoTracking()
                                .FirstOrDefaultAsync(r => r.ResourceVersionId == resourceVersionId && r.Id == authorId);
        }
    }
}
