// <copyright file="FolderNodeVersionRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Hierarchy
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The FolderNodeVersionRepository.
    /// </summary>
    public class FolderNodeVersionRepository : GenericRepository<FolderNodeVersion>, IFolderNodeVersionRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FolderNodeVersionRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public FolderNodeVersionRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetFolder.
        /// </summary>
        /// <param name="nodeVersionId">The node version id.</param>
        /// <returns>The folder node version.</returns>
        public async Task<FolderNodeVersion> GetFolderAsync(int nodeVersionId)
        {
            var folder = await this.DbContext.FolderNodeVersion.AsNoTracking()
                    .Include(f => f.NodeVersion)
                    .ThenInclude(f => f.Node)
                    .FirstOrDefaultAsync(f => f.NodeVersionId == nodeVersionId);

            return folder;
        }
    }
}
