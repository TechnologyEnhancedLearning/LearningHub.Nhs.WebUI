// <copyright file="ScormResourceReferenceEventRepository.cs" company="HEE.nhs.uk">
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
    public class ScormResourceReferenceEventRepository : GenericRepository<ScormResourceReferenceEvent>, IScormResourceReferenceEventRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScormResourceReferenceEventRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext<see cref="LearningHubDbContext"/>.</param>
        /// <param name="tzOffsetManager">tzOffsetManager.</param>
        public ScormResourceReferenceEventRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
