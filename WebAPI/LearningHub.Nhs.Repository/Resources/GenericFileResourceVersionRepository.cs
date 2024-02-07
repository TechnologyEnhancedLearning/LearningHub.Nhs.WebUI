// <copyright file="GenericFileResourceVersionRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The generic file resource version repository.
    /// </summary>
    public class GenericFileResourceVersionRepository : GenericRepository<GenericFileResourceVersion>, IGenericFileResourceVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFileResourceVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public GenericFileResourceVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<GenericFileResourceVersion> GetByIdAsync(int id)
        {
            return await this.DbContext.GenericFileResourceVersion.AsNoTracking().FirstOrDefaultAsync(r => r.Id == id && !r.Deleted);
        }

        /// <summary>
        /// The get by resource version id async.
        /// </summary>
        /// <param name="resourceVersionid">The resource versionid.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<GenericFileResourceVersion> GetByResourceVersionIdAsync(int resourceVersionid, bool includeDeleted = false)
        {
            return await this.DbContext.GenericFileResourceVersion
                .Include(gfrv => gfrv.File).ThenInclude(f => f.FileType)
                .AsNoTracking().FirstOrDefaultAsync(gfrv => gfrv.ResourceVersionId == resourceVersionid && (includeDeleted || !gfrv.Deleted));
        }

        /// <summary>
        /// The get by resource version id.
        /// </summary>
        /// <param name="resourceVersionid">The resource versionid.</param>
        /// <param name="includeDeleted">Allows deleted items to be returned.</param>
        /// <returns>The generic file resource version.</returns>
        public GenericFileResourceVersion GetByResourceVersionId(int resourceVersionid, bool includeDeleted = false)
        {
            return this.DbContext.GenericFileResourceVersion
                .Include(gfrv => gfrv.File).ThenInclude(f => f.FileType)
                .AsNoTracking().FirstOrDefault(gfrv => gfrv.ResourceVersionId == resourceVersionid && (includeDeleted || !gfrv.Deleted));
        }
    }
}
