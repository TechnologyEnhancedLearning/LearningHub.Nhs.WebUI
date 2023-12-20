// <copyright file="InternalSystemRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Maintenance
{
    using LearningHub.Nhs.Models.Entities.Maintenance;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Maintenance;

    /// <summary>
    /// The InternalSystemRepository.
    /// </summary>
    public class InternalSystemRepository : GenericRepository<InternalSystem>, IInternalSystemRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InternalSystemRepository"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="tzOffsetManager">
        /// The Timezone offset manager.
        /// </param>
        public InternalSystemRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
