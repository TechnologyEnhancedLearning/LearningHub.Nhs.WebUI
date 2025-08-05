namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories.Resources;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;

    /// <summary>
    /// The ResourceReferenceService class.
    /// </summary>
    public class ResourceReferenceService : IResourceReferenceService
    {
        private readonly IResourceReferenceRepository resourceReferenceRepo;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReferenceService"/> class.
        /// </summary>
        /// <param name="resourceReferenceRepo">The resourceReferenceRepo.</param>
        public ResourceReferenceService(IResourceReferenceRepository resourceReferenceRepo)
        {
            this.resourceReferenceRepo = resourceReferenceRepo;
        }

        /// <inheritdoc/>
        public async Task<ResourceReference> GetByIdAsync(int id)
        {
            return await resourceReferenceRepo.GetByIdAsync(id, false);
        }
    }
}