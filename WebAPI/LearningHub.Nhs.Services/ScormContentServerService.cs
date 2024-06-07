namespace LearningHub.Nhs.Services
{
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Api.Shared.Configuration;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Resource;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// The ScormContentServerService.
    /// </summary>
    public class ScormContentServerService : IScormContentServerService
    {
        /// <summary>
        /// Defines the SystemUserId.
        /// </summary>
        private const int SystemUserId = 4;

        /// <summary>
        /// The scormResourceVersionRepository..
        /// </summary>
        private IScormResourceVersionRepository scormResourceVersionRepository;

        /// <summary>
        /// The resourceReferenceEventRepository..
        /// </summary>
        private IResourceReferenceEventRepository resourceReferenceEventRepository;

        /// <summary>
        /// Defines the mapper.
        /// </summary>
        private IMapper mapper;

        /// <summary>
        /// The settings......
        /// </summary>
        private Settings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="ScormContentServerService"/> class.
        /// </summary>
        /// <param name="scormResourceVersionRepository">The scormResourceVersionRepository<see cref="IScormResourceVersionRepository"/>.</param>
        /// <param name="resourceReferenceEventRepository">The resourceReferenceEventRepository.</param>
        /// <param name="mapper">mapper.</param>
        /// <param name="settings">The settings.</param>
        public ScormContentServerService(
            IScormResourceVersionRepository scormResourceVersionRepository,
            IResourceReferenceEventRepository resourceReferenceEventRepository,
            IMapper mapper,
            IOptions<Settings> settings)
        {
            this.scormResourceVersionRepository = scormResourceVersionRepository;
            this.resourceReferenceEventRepository = resourceReferenceEventRepository;
            this.mapper = mapper;
            this.settings = settings.Value;
        }

        /// <summary>
        /// Gets the SCORM content details for a particular external url (LH or historic).
        /// </summary>
        /// <param name="externalUrl">The externalUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ContentServerViewModel"/>.</returns>
        public ContentServerViewModel GetContentDetailsByExternalUrl(string externalUrl)
        {
            var response = this.scormResourceVersionRepository.GetScormContentServerDetailsByHistoricExternalUrl(externalUrl);

            return response;
        }

        /// <summary>
        /// The GetScormContentDetailsByExternalReference.
        /// </summary>
        /// <param name="externalReference">The externalReference<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ContentServerViewModel}"/>.</returns>
        public ContentServerViewModel GetContentDetailsByExternalReference(string externalReference)
        {
            var response = this.scormResourceVersionRepository.GetContentServerDetailsByLHExternalReference(externalReference);

            return response;
        }

        /// <summary>
        /// The LogResourceReferenceEventAsync.
        /// </summary>
        /// <param name="resourceReferenceEventViewModel">The resourceReferenceEventViewModel<see cref="ResourceReferenceEventViewModel"/>.</param>
        public void LogResourceReferenceEventAsync(ResourceReferenceEventViewModel resourceReferenceEventViewModel)
        {
             var resourceReferenceEvent = this.mapper.Map<ResourceReferenceEvent>(resourceReferenceEventViewModel);
             var id = this.resourceReferenceEventRepository.CreateAsync(SystemUserId, resourceReferenceEvent);
        }
    }
}
