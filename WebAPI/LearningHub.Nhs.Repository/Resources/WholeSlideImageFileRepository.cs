// <copyright file="WholeSlideImageFileRepository.cs" company="NHS England">
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
    public class WholeSlideImageFileRepository : GenericRepository<WholeSlideImageFile>, IWholeSlideImageFileRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WholeSlideImageFileRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public WholeSlideImageFileRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by File Id async.
        /// </summary>
        /// <param name="fileId">The File Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<WholeSlideImageFile> GetByFileIdAsync(int fileId)
        {
            return await this.DbContext.WholeSlideImageFile.AsNoTracking().FirstOrDefaultAsync(f => f.FileId == fileId && !f.Deleted);
        }
    }
}
