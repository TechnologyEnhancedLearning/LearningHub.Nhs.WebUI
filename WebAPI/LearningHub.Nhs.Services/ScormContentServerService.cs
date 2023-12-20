// <copyright file="ScormContentServerService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

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
        /// The scormResourceReferenceEventRepository..
        /// </summary>
        private IScormResourceReferenceEventRepository scormResourceReferenceEventRepository;

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
        /// <param name="scormResourceReferenceEventRepository">The scormResourceReferenceEventRepository.</param>
        /// <param name="mapper">mapper.</param>
        /// <param name="settings">The settings.</param>
        public ScormContentServerService(
            IScormResourceVersionRepository scormResourceVersionRepository,
            IScormResourceReferenceEventRepository scormResourceReferenceEventRepository,
            IMapper mapper,
            IOptions<Settings> settings)
        {
            this.scormResourceVersionRepository = scormResourceVersionRepository;
            this.scormResourceReferenceEventRepository = scormResourceReferenceEventRepository;
            this.mapper = mapper;
            this.settings = settings.Value;
        }

        /// <summary>
        /// Gets the SCORM content details for a particular external url (LH or historic).
        /// </summary>
        /// <param name="externalUrl">The externalUrl<see cref="string"/>.</param>
        /// <returns>The <see cref="ScormContentServerViewModel"/>.</returns>
        public ScormContentServerViewModel GetScormContentDetailsByExternalUrl(string externalUrl)
        {
            var response = this.scormResourceVersionRepository.GetScormContentServerDetailsByHistoricExternalUrl(externalUrl);

            return response;
        }

        /// <summary>
        /// The GetScormContentDetailsByExternalReference.
        /// </summary>
        /// <param name="externalReference">The externalReference<see cref="string"/>.</param>
        /// <returns>The <see cref="Task{ScormContentServerViewModel}"/>.</returns>
        public ScormContentServerViewModel GetScormContentDetailsByExternalReference(string externalReference)
        {
            var response = this.scormResourceVersionRepository.GetScormContentServerDetailsByLHExternalReference(externalReference);

            return response;
        }

        /// <summary>
        /// The LogScormResourceReferenceEventAsync.
        /// </summary>
        /// <param name="scormResourceReferenceEventViewModel">The scormResourceReferenceEventViewModel<see cref="ScormResourceReferenceEventViewModel"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task LogScormResourceReferenceEventAsync(ScormResourceReferenceEventViewModel scormResourceReferenceEventViewModel)
        {
             var scormResourceReferenceEvent = this.mapper.Map<ScormResourceReferenceEvent>(scormResourceReferenceEventViewModel);
             var id = await this.scormResourceReferenceEventRepository.CreateAsync(SystemUserId, scormResourceReferenceEvent);
        }
    }
}
