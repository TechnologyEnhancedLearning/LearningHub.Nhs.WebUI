namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;

    /// <summary>
    /// The Resource Reference Service interface.
    /// </summary>
    public interface IResourceReferenceService
    {
        /// <summary>
        /// The get resource reference by id async.
        /// </summary>
        /// <param name="id">The id<see cref="int"/>.</param>
        /// <returns>The <see cref="Task{ResourceReference}"/>.</returns>
        Task<ResourceReference> GetByIdAsync(int id);
    }
}