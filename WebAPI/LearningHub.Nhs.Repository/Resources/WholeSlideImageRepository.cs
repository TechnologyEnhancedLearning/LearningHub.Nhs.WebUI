// <copyright file="WholeSlideImageRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The whole slide image repository.
    /// </summary>
    public class WholeSlideImageRepository : GenericRepository<WholeSlideImage>, IWholeSlideImageRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WholeSlideImageRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public WholeSlideImageRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by fileId async.
        /// </summary>
        /// <param name="fileId"> The wholeSlideImageBlockId. </param>
        /// <returns>The <see cref="Task{TResult}"/>. </returns>
        public async Task<WholeSlideImage> GetByFileIdAsync(int fileId)
        {
            return await this.DbContext.WholeSlideImage.Include(wsi => wsi.WholeSlideImageBlockItems)
                .ThenInclude(wsibi => wsibi.WholeSlideImageBlock)
                .ThenInclude(wsib => wsib.Block)
                .ThenInclude(b => b.BlockCollection)
                .ThenInclude(bc => bc.CaseResourceVersion)
                .AsNoTracking().FirstOrDefaultAsync(wsi => wsi.FileId == fileId);
        }
    }
}