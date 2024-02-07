// <copyright file="ScormResourceVersionManifestRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The Scorm resource version manifest repository.
    /// </summary>
    public class ScormResourceVersionManifestRepository : GenericRepository<ScormResourceVersionManifest>, IScormResourceVersionManifestRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScormResourceVersionManifestRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ScormResourceVersionManifestRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<ScormResourceVersionManifest> GetByScormResourceVersionIdAsync(int id)
        {
            return await this.DbContext.ScormResourceVersionManifest.AsNoTracking().FirstOrDefaultAsync(r => r.ScormResourceVersionId == id && !r.Deleted);
        }
    }
}
