namespace LearningHub.Nhs.Repository.Interface.Hierarchy
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// The IFolderNodeVersionRepository.
    /// </summary>
    public interface IFolderNodeVersionRepository : IGenericRepository<FolderNodeVersion>
    {
        /// <summary>
        /// The GetFolder.
        /// </summary>
        /// <param name="nodeVersionId">The node version id.</param>
        /// <returns>The folder node version.</returns>
        Task<FolderNodeVersion> GetFolderAsync(int nodeVersionId);
    }
}
