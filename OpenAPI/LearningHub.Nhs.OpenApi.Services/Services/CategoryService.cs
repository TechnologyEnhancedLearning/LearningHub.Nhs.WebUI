namespace LearningHub.Nhs.OpenApi.Services.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoMapper;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Provider;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using LearningHub.Nhs.OpenApi.Services.Interface.Services;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The provider service.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        /// <summary>
        /// The provider repository.
        /// </summary>
        private readonly ICategoryRepository categoryRepository;

        /// <summary>
        ///  mapper.
        /// </summary>
        private readonly IMapper mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="categoryRepository">The category repository.</param>
        /// <param name="mapper">The mapper.</param>
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            this.categoryRepository = categoryRepository;
            this.mapper = mapper;
        }

        /// <inheritdoc />
        public async Task<int> GetByCatalogueVersionIdAsync(int nodeVersionId)
        {
            var category = await categoryRepository.GetCategoryByCatalogueIdAsync(nodeVersionId);
            return category != null ? category.CategoryId : 0;
        }
    }
}