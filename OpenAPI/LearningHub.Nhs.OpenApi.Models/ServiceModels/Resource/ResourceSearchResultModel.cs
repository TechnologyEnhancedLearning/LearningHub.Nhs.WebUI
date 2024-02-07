// <copyright file="ResourceSearchResultModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource
{
    using System.Collections.Generic;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Findwise;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;

    /// <summary>
    /// <see cref="ResourceSearchResultViewModel"/>.
    /// </summary>
    public class ResourceSearchResultModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSearchResultModel"/> class.
        /// </summary>
        /// <param name="resources"><see cref="Resources"/>.</param>
        /// <param name="findwiseRequestStatus"><see cref="FindwiseRequestStatus"/>.</param>
        /// <param name="totalNumResources"><see cref="TotalNumResources"/>.</param>
        public ResourceSearchResultModel(
            List<ResourceMetadataViewModel> resources,
            FindwiseRequestStatus findwiseRequestStatus,
            int totalNumResources)
        {
            this.Resources = resources;
            this.FindwiseRequestStatus = findwiseRequestStatus;
            this.TotalNumResources = totalNumResources;
        }

        /// <summary>
        /// Gets <see cref="Resources"/>.
        /// </summary>
        public List<ResourceMetadataViewModel> Resources { get; }

        /// <summary>
        /// Gets <see cref="FindwiseRequestStatus"/>.
        /// </summary>
        public FindwiseRequestStatus FindwiseRequestStatus { get; }

        /// <summary>
        /// Gets <see cref="TotalNumResources"/>.
        /// </summary>
        public int TotalNumResources { get; }

        /// <summary>
        /// Failed result.
        /// </summary>
        /// <param name="status">Status.</param>
        /// <returns><see cref="ResourceSearchResultModel"/>.</returns>
        public static ResourceSearchResultModel FailedWithStatus(FindwiseRequestStatus status)
        {
            return new (new List<ResourceMetadataViewModel>(), status, 0);
        }
    }
}
