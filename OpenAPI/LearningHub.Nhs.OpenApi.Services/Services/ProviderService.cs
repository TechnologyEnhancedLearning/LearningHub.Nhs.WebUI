namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
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
            var providers = await providerRepository.GetAll().ToListAsync();
            return mapper.Map<List<ProviderViewModel>>(providers);
        }

        /// <inheritdoc />
        public async Task<ProviderViewModel> GetByIdAsync(int id)
        {
            var provider = await providerRepository.GetByIdAsync(id);
            return mapper.Map<ProviderViewModel>(provider);
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetByUserIdAsync(int userId)
        {
            var providers = await providerRepository.GetProvidersByUserIdAsync(userId).ToListAsync();
            return mapper.Map<List<ProviderViewModel>>(providers);
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetByResourceVersionIdAsync(int resourceVersionId)
        {
            var providers = await providerRepository.GetProvidersByResourceIdAsync(resourceVersionId).ToListAsync();
            return mapper.Map<List<ProviderViewModel>>(providers);
        }

        /// <inheritdoc />
        public async Task<List<ProviderViewModel>> GetByCatalogueVersionIdAsync(int nodeVersionId)
        {
            var providers = await providerRepository.GetProvidersByCatalogueIdAsync(nodeVersionId).ToListAsync();
            return mapper.Map<List<ProviderViewModel>>(providers);
        }
    }
}