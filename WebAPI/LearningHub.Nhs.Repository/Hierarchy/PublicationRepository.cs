namespace LearningHub.Nhs.Repository.Hierarchy
{
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using Microsoft.Data.SqlClient;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The publication repository.
    /// </summary>
    public class PublicationRepository : GenericRepository<Publication>, IPublicationRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicationRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public PublicationRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<Publication> GetByIdAsync(int id)
        {
            return await this.DbContext.Publication.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The Publication.</returns>
        public Publication GetById(int id)
        {
            return this.DbContext.Publication.AsNoTracking().FirstOrDefault(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by id.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <returns>The Publication.</returns>
        public async Task<Publication> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            return await this.DbContext.Publication.AsNoTracking().FirstOrDefaultAsync(r => r.ResourceVersionId == resourceVersionId && !r.Deleted);
        }

        /// <summary>
        /// Get cache operations for the supplied publication id.
        /// </summary>
        /// <param name="publicationId">The publicationId.</param>
        /// <returns>A list of <see cref="CacheOperationViewModel"/>.</returns>
        public async Task<List<CacheOperationViewModel>> GetCacheOperations(int publicationId)
        {
            var param0 = new SqlParameter("@p0", SqlDbType.Int) { Value = publicationId };

            var vm = await this.DbContext.CacheOperationViewModel.FromSqlRaw("hierarchy.GetCacheRefreshDetails @p0", param0).ToListAsync();

            return vm;
        }

        /// <summary>
        /// Get resource first publication record.
        /// </summary>
        /// <param name="resourceId">resource id.</param>
        /// <returns>publish view model.</returns>
        public async Task<Publication> GetResourceFirstPublication(int resourceId)
        {
            return await (from pub in this.DbContext.Publication.AsNoTracking()
                          join rv in this.DbContext.ResourceVersion on pub.ResourceVersionId equals rv.Id
                          join r in this.DbContext.Resource on rv.ResourceId equals r.Id
                          where r.Id == resourceId
                          select pub).OrderBy(n => n.CreateDate).FirstOrDefaultAsync();
        }
    }
}
