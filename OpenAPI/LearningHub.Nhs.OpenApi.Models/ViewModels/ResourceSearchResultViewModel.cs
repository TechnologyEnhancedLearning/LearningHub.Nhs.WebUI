// <copyright file="ResourceSearchResultViewModel.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    using System.Collections.Generic;
    using LearningHub.Nhs.OpenApi.Models.ServiceModels.Resource;

    /// <summary>
    /// <see cref="ResourceSearchResultViewModel"/>.
    /// </summary>
    public class ResourceSearchResultViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceSearchResultViewModel"/> class.
        /// </summary>
        /// <param name="results"><see cref="Results"/>.</param>
        /// <param name="offset"><see cref="Offset"/>.</param>
        public ResourceSearchResultViewModel(ResourceSearchResultModel results, int offset)
        {
            this.Results = results.Resources;
            this.Offset = offset;
            this.TotalNumResources = results.TotalNumResources;
        }

        /// <summary>
        /// Gets or sets Results.
        /// </summary>
        public List<ResourceMetadataViewModel> Results { get; set; }

        /// <summary>
        /// Gets or sets Offset.
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Gets or sets TotalNumResources.
        /// </summary>
        public int TotalNumResources { get; set; }
    }
}
