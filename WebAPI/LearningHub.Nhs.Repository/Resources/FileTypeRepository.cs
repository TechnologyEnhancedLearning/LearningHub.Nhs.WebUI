// <copyright file="FileTypeRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;

    /// <summary>
    /// The file type repository.
    /// </summary>
    public class FileTypeRepository : GenericRepository<FileType>, IFileTypeRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileTypeRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public FileTypeRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public Task<FileType> GetByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}
