// <copyright file="ResourceReferenceEventRepository.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;

    /// <summary>
    /// The scorm resource version Event repository.
    /// </summary>
    public class ResourceReferenceEventRepository : GenericRepository<ResourceReferenceEvent>, IResourceReferenceEventRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceReferenceEventRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext<see cref="LearningHubDbContext"/>.</param>
        /// <param name="tzOffsetManager">tzOffsetManager.</param>
        public ResourceReferenceEventRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
