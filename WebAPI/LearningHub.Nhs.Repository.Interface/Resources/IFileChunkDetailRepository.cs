namespace LearningHub.Nhs.Repository.Interface.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The FileChunkDetailRepository interface.
    /// </summary>
    public interface IFileChunkDetailRepository : IGenericRepository<FileChunkDetail>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<FileChunkDetail> GetByIdAsync(int id);

        /// <summary>
        /// Delete a file chunk detail.
        /// </summary>
        /// <param name="fileChunkDetailId">The file Chunk Detail id.</param>
        /// <param name="userId">The user id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task Delete(int fileChunkDetailId, int userId);
    }
}
