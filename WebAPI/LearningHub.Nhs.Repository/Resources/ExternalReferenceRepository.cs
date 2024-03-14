namespace LearningHub.Nhs.Repository.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;

    /// <summary>
    /// The external reference repository.
    /// </summary>
    public class ExternalReferenceRepository : GenericRepository<ExternalReference>, IExternalReferenceRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalReferenceRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The db context.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ExternalReferenceRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
