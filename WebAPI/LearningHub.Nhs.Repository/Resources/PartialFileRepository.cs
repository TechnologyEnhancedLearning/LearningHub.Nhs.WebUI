// <copyright file="PartialFileRepository.cs" company="NHS England">
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
    /// The partial file repository.
    /// </summary>
    public class PartialFileRepository : GenericRepository<PartialFile>, IPartialFileRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartialFileRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public PartialFileRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by File Id async.
        /// </summary>
        /// <param name="fileId">The File Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<PartialFile> GetByFileIdAsync(int fileId)
        {
            return await this.DbContext.PartialFile.AsNoTracking().FirstOrDefaultAsync(f => f.FileId == fileId && !f.Deleted);
        }
    }
}
