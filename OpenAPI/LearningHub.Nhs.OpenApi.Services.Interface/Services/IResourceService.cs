// <copyright file="IResourceService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;

    /// <summary>
    /// The ResourceService interface.
    /// </summary>
    public interface IResourceService
    {
        /// <summary>
        /// The get resource by id async.
        /// </summary>
        /// <param name="originalResourceReferenceId">The original resource reference id.</param>
        /// <returns>The <see cref="Task"/>the resourceMetaDataViewModel corresponding to the resource reference.</returns>
        Task<ResourceReferenceWithResourceDetailsViewModel> GetResourceReferenceByOriginalId(int originalResourceReferenceId);

        /// <summary>
        /// The get resources by Ids endpoint.
        /// </summary>
        /// <param name="originalResourceReferenceIds">The original resource reference Ids.</param>
        /// <returns><see cref="Task"/>The resourceReferenceMetaDataViewModel.</returns>
        Task<BulkResourceReferenceViewModel> GetResourceReferencesByOriginalIds(List<int> originalResourceReferenceIds);
    }
}
