// <copyright file="CatalogueNodeVersionProviderRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;

    /// <summary>
    /// The CatalogueNodeVersionProviderRepository.
    /// </summary>
    public class CatalogueNodeVersionProviderRepository : GenericRepository<CatalogueNodeVersionProvider>, ICatalogueNodeVersionProviderRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueNodeVersionProviderRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public CatalogueNodeVersionProviderRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
