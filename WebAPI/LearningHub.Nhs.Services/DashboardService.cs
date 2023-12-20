// <copyright file="DashboardService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Dashboard;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using LearningHub.Nhs.Repository.Interface.Resources;
    using LearningHub.Nhs.Services.Interface;

    /// <summary>
    /// The DashboardService.
    /// </summary>
    public class DashboardService : IDashboardService
    {
        private readonly IMapper mapper;
        private IResourceVersionRepository resourceVersionRepository;
        private ICatalogueNodeVersionRepository catalogueNodeVersionRepository;
        private IRatingService ratingService;
        private IProviderService providerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardService"/> class.
        /// </summary>
        /// <param name="mapper">mapper.</param>
        /// <param name="resourceVersionRepository">resourceVersionRepository.</param>
        /// <param name="catalogueNodeVersionRepository">catalogueNodeVersionRepository.</param>
        /// <param name="ratingService">ratingService.</param>
        /// <param name="providerService">providerService.</param>
        public DashboardService(IMapper mapper, IResourceVersionRepository resourceVersionRepository, ICatalogueNodeVersionRepository catalogueNodeVersionRepository, IRatingService ratingService, IProviderService providerService)
        {
            this.mapper = mapper;
            this.resourceVersionRepository = resourceVersionRepository;
            this.catalogueNodeVersionRepository = catalogueNodeVersionRepository;
            this.ratingService = ratingService;
            this.providerService = providerService;
        }

        /// <summary>
        /// GetMyAccessLearnings.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardMyLearningResponseViewModel> GetMyAccessLearnings(string dashboardType, int pageNumber, int userId)
        {
            var (resourceCount, resources) = dashboardType.ToLower() != "my-catalogues" ? this.resourceVersionRepository.GetResources(dashboardType, pageNumber, userId) : (resourceCount: 0, resources: new List<DashboardResourceDto>());

            var cataloguesResponse = dashboardType.ToLower() == "my-catalogues" ? this.catalogueNodeVersionRepository.GetCatalogues(dashboardType, pageNumber, userId) : (TotalCount: 0, Catalogues: new List<DashboardCatalogueDto>());

            var resourceList = resources.Any() ? this.mapper.Map<List<DashboardResourceViewModel>>(resources) : new List<DashboardResourceViewModel>();
            if (resourceList.Any())
            {
                foreach (var resource in resourceList)
                {
                    resource.Providers = await this.providerService.GetByResourceVersionIdAsync(resource.ResourceVersionId);
                }
            }

            var response = new DashboardMyLearningResponseViewModel
            {
                Type = dashboardType,
                Resources = resourceList,
                Catalogues = cataloguesResponse.Catalogues.Any() ? this.mapper.Map<List<DashboardCatalogueViewModel>>(cataloguesResponse.Catalogues) : new List<DashboardCatalogueViewModel>(),
                TotalCount = dashboardType.ToLower() == "my-catalogues" ? cataloguesResponse.TotalCount : resourceCount,
                CurrentPage = pageNumber,
            };

            return response;
        }

        /// <summary>
        /// GetCatalogues.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The page Number.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardCatalogueResponseViewModel> GetCatalogues(string dashboardType, int pageNumber, int userId)
        {
            var (catalogueCount, catalogues) = this.catalogueNodeVersionRepository.GetCatalogues(dashboardType, pageNumber, userId);

            var catalogueList = catalogues.Any() ? this.mapper.Map<List<DashboardCatalogueViewModel>>(catalogues) : new List<DashboardCatalogueViewModel>();
            foreach (var catalogue in catalogueList)
            {
                catalogue.Providers = await this.providerService.GetByCatalogueVersionIdAsync(catalogue.NodeVersionId);
            }

            var response = new DashboardCatalogueResponseViewModel
            {
                Type = dashboardType,
                Catalogues = catalogueList,
                TotalCount = catalogueCount,
                CurrentPage = pageNumber,
            };
            return response;
        }

        /// <summary>
        /// GetResources.
        /// </summary>
        /// <param name="dashboardType">The dashboard type.</param>
        /// <param name="pageNumber">The number of rows to return.</param>
        /// <param name="userId">The userId.</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        public async Task<DashboardResourceResponseViewModel> GetResources(string dashboardType, int pageNumber, int userId)
        {
            var (resourceCount, resources) = this.resourceVersionRepository.GetResources(dashboardType, pageNumber, userId);

            var resourceList = resources.Any() ? this.mapper.Map<List<DashboardResourceViewModel>>(resources) : new List<DashboardResourceViewModel>();

            foreach (var resource in resourceList)
            {
                resource.Providers = await this.providerService.GetByResourceVersionIdAsync(resource.ResourceVersionId);
            }

            var response = new DashboardResourceResponseViewModel
            {
                Type = dashboardType,
                Resources = resourceList,
                TotalCount = resourceCount,
                CurrentPage = pageNumber,
            };

            return response;
        }
    }
}
