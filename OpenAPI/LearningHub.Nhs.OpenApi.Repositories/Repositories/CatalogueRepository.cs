// <copyright file="CatalogueRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Repositories
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.OpenApi.Repositories.EntityFramework;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Repositories;
    using Microsoft.EntityFrameworkCore;

    /// <inheritdoc />
    public class CatalogueRepository : ICatalogueRepository
    {
        private LearningHubDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="CatalogueRepository"/> class.
        /// </summary>
        /// <param name="dbContext"><see cref="dbContext"/>.</param>
        public CatalogueRepository(LearningHubDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CatalogueNodeVersion>> GetAllCatalogues()
        {
            return await this.dbContext.CatalogueNodeVersion
                .Include(cnv => cnv.NodeVersion)
                .ThenInclude(nv => nv.Node)
                .Where(cnv => cnv.NodeVersion == cnv.NodeVersion.Node.CurrentNodeVersion)
                .ToListAsync();
        }
    }
}