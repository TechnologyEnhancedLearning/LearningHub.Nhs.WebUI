// <copyright file="PublicationLogRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Hierarchy;

    /// <summary>
    /// The publication log repository.
    /// </summary>
    public class PublicationLogRepository : GenericRepository<PublicationLog>, IPublicationLogRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicationLogRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public PublicationLogRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
