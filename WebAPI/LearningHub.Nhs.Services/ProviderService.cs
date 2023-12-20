// <copyright file="ProviderService.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Services.Interface;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The provider service.
    /// </summary>
    public class ProviderService : IProviderService
    {
        /// <summary>
        /// The provider repository.
        /// </summary>
        private readonly IProviderRepository providerRepository;

        /// <summary>
        ///  mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderService"/> class.
        /// </summary>
        /// <param name="providerRepository">The provider repository.</param>
        /// <param name="mapper">The mapper.</param>
        public ProviderService(IProviderRepository providerRepository, IMapper mapper)
        {
            this.providerRepository = providerRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetAllAsync()
        {
            var providers = await this.providerRepository.GetAll().ToListAsync();
            return this.mapper.Map<List<ProviderViewModel>>(providers);
        }

        /// <inheritdoc />
        public async Task<ProviderViewModel> GetByIdAsync(int id)
        {
            var provider = await this.providerRepository.GetByIdAsync(id);
            return this.mapper.Map<ProviderViewModel>(provider);
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetByUserIdAsync(int userId)
        {
            var providers = await this.providerRepository.GetProvidersByUserIdAsync(userId).ToListAsync();
            return this.mapper.Map<List<ProviderViewModel>>(providers);
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            var providers = await this.providerRepository.GetProvidersByResourceIdAsync(resourceVersionId).ToListAsync();
            return this.mapper.Map<List<ProviderViewModel>>(providers);
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetByCatalogueVersionIdAsync(int nodeVersionId)
        {
            var providers = await this.providerRepository.GetProvidersByCatalogueIdAsync(nodeVersionId).ToListAsync();
            return this.mapper.Map<List<ProviderViewModel>>(providers);
        }
    }
}