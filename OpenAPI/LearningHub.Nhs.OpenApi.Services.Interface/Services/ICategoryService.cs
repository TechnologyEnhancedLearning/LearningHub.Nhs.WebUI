namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;

    /// <summary>
    /// The Category Service interface.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Get category by node version id.
        /// </summary>
        /// <param name="nodeVersionId">node version id.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        Task<int> GetByCatalogueVersionIdAsync(int nodeVersionId);
    }
}
