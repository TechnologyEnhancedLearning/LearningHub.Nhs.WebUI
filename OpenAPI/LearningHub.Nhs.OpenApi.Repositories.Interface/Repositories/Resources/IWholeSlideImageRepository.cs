namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;

    /// <summary>
    /// The whole slide image repository interface.
    /// </summary>
    public interface IWholeSlideImageRepository : IGenericRepository<WholeSlideImage>
    {
        /// <summary>
        /// The get by fileId async.
        /// </summary>
        /// <param name="fileId">The file Id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<WholeSlideImage> GetByFileIdAsync(int fileId);
    }
}