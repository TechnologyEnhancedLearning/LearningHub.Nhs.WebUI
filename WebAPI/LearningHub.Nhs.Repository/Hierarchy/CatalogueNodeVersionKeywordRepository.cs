// <copyright file="CatalogueNodeVersionKeywordRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;

    /// <summary>
    /// The CatalogueNodeVersionKeywordRepository.
    /// </summary>
    public class CatalogueNodeVersionKeywordRepository : GenericRepository<CatalogueNodeVersionKeyword>, ICatalogueNodeVersionKeywordRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueNodeVersionKeywordRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public CatalogueNodeVersionKeywordRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
