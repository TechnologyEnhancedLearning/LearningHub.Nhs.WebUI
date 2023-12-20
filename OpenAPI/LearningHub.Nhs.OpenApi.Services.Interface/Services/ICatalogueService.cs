// <copyright file="ICatalogueService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    using System.Threading.Tasks;
    using LearningHub.Nhs.OpenApi.Models.ViewModels;

    /// <summary>
    /// The CatalogueService interface.
    /// </summary>
    public interface ICatalogueService
    {
        /// <summary>
        /// get all catalogues async.
        /// </summary>
        /// <returns>BulkCatalogueViewModel.</returns>
        public Task<BulkCatalogueViewModel> GetAllCatalogues();
    }
}