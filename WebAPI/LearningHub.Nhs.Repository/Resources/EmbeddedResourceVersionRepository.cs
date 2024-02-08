namespace LearningHub.Nhs.Repository.Resources
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The embedded resource version repository.
    /// </summary>
    public class EmbeddedResourceVersionRepository : GenericRepository<EmbeddedResourceVersion>, IEmbeddedResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddedResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public EmbeddedResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<EmbeddedResourceVersion> GetByIdAsync(int id)
        {
            return await this.DbContext.EmbeddedResourceVersion.AsNoTracking()
                                .Include(rv => rv.ResourceVersion).AsNoTracking()
                                .FirstOrDefaultAsync(r => r.Id == id && !r.Deleted && !r.ResourceVersion.Deleted);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionId">The resource version id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<EmbeddedResourceVersion> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return await this.DbContext.EmbeddedResourceVersion.AsNoTracking()
                                .Include(rv => rv.ResourceVersion)
                                .ThenInclude(rv => rv.Resource).AsNoTracking()
                                .FirstOrDefaultAsync(r => r.ResourceVersionId == resourceVersionId && !r.Deleted && !r.ResourceVersion.Deleted);
        }

        /// <summary>
        /// The update async.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="embeddedResourceVersion">The embedded resource version.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async new Task UpdateAsync(int userId, EmbeddedResourceVersion embeddedResourceVersion)
        {
            var amendDate = DateTimeOffset.Now;

            var embeddedResourceVersionUpdate = this.DbContext.EmbeddedResourceVersion
                                                            .Where(r => r.ResourceVersionId == embeddedResourceVersion.ResourceVersionId
                                                                    && !r.Deleted)
                                                            .SingleOrDefault();

            if (embeddedResourceVersionUpdate != null)
            {
                embeddedResourceVersionUpdate.EmbedCode = embeddedResourceVersion.EmbedCode;
                this.SetAuditFieldsForUpdate(userId, embeddedResourceVersionUpdate);
            }

            await this.DbContext.SaveChangesAsync();
        }
    }
}
