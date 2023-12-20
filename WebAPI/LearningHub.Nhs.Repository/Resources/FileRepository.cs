// <copyright file="FileRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The file repository.
    /// </summary>
    public class FileRepository : GenericRepository<File>, IFileRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public FileRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<File> GetByIdAsync(int id)
        {
            return await this.DbContext.File.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id && !f.Deleted);
        }
    }
}
