// <copyright file="RoadmapRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository
{
    using System.Collections.Generic;
    using System.Linq;
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Interface;

    /// <summary>
    /// The RoadmapRepository.
    /// </summary>
    public class RoadmapRepository : GenericRepository<Roadmap>, IRoadmapRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoadmapRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public RoadmapRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }

        /// <summary>
        /// The GetUpdates.
        /// </summary>
        /// <returns>The updates.</returns>
        public List<Roadmap> GetUpdates()
        {
            return this.GetAll().Where(x => x.RoadmapTypeId == 1).ToList();
        }
    }
}
