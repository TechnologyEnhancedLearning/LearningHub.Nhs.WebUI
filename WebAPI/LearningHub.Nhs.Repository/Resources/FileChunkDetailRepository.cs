namespace LearningHub.Nhs.Repository.Resources
{
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The file chunk header repository.
    /// </summary>
    public class FileChunkDetailRepository : GenericRepository<FileChunkDetail>, IFileChunkDetailRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileChunkDetailRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public FileChunkDetailRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<FileChunkDetail> GetByIdAsync(int id)
        {
            return await this.DbContext.FileChunkDetail.AsNoTracking().FirstOrDefaultAsync(f => f.Id == id && !f.Deleted);
        }

        /// <summary>
        /// Deletes a file chunk detail.
        /// </summary>
        /// <param name="fileChunkDetailId">The file chunk detail id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task Delete(int fileChunkDetailId, int userId)
        {
            var fileChunkDetail = this.DbContext.FileChunkDetail.SingleOrDefault(fcd => fcd.Id == fileChunkDetailId && !fcd.Deleted);

            if (fileChunkDetail != null)
            {
                fileChunkDetail.Deleted = true;
                fileChunkDetail.AmendUserId = userId;
                await this.UpdateAsync(userId, fileChunkDetail);
            }
        }
    }
}
