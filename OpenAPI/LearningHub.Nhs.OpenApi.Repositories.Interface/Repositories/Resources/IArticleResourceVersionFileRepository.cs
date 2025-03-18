namespace LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The ArticleResourceVersionFileRepository interface.
    /// </summary>
    public interface IArticleResourceVersionFileRepository : IGenericRepository<ArticleResourceVersionFile>
    {
        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ArticleResourceVersionFile> GetByIdAsync(int id);

        /// <summary>
        /// The get by id async.
        /// </summary>
        /// <param name="resourceVersionId">The resourceVersionId.</param>
        /// <param name="fileId">The fileId.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<ArticleResourceVersionFile> GetByResourceVersionAndFileAsync(int resourceVersionId, int fileId);
    }
}
