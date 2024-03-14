// <copyright file="NodeActivityRepository.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>
namespace LearningHub.Nhs.Repository.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Activity;

    /// <summary>
    /// The node activity repository.
    /// </summary>
    public class NodeActivityRepository : GenericRepository<NodeActivity>, INodeActivityRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeActivityRepository"/> class.
        /// </summary>
        /// <param name="dbContext">dbContext.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public NodeActivityRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
