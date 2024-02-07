// <copyright file="NodeResourceLookupRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Hierarchy
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The node resource repository.
    /// </summary>
    public class NodeResourceLookupRepository : GenericRepository<NodeResourceLookup>, INodeResourceLookupRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeResourceLookupRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public NodeResourceLookupRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The get by node id async.
        /// </summary>
        /// <param name="nodeId">The node id.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public async Task<List<NodeResourceLookup>> GetByNodeIdAsync(int nodeId)
        {
            return await this.DbContext.NodeResourceLookup.AsNoTracking()
                                                    .Where(nrl => nrl.NodeId == nodeId)
                                                    .ToListAsync();
        }
    }
}
