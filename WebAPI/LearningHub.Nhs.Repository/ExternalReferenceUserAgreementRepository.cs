namespace LearningHub.Nhs.Repository
{
    using LearningHub.Nhs.Entities.Resource;
    using LearningHub.Nhs.Repository.Interface;
    using LearningHub.Nhs.Repository.Interface.Resources;

    /// <summary>
    /// Defines the <see cref="ExternalReferenceUserAgreementRepository" />.
    /// </summary>
    public class ExternalReferenceUserAgreementRepository : GenericRepository<ExternalReferenceUserAgreement>, IExternalReferenceUserAgreementRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExternalReferenceUserAgreementRepository"/> class.
        /// </summary>
        /// <param name="dbContext">The dbContext<see cref="LearningHubDbContext"/>.</param>
        /// <param name="tzOffsetManager">The Timezone offset manager.</param>
        public ExternalReferenceUserAgreementRepository(LearningHubDbContext dbContext, ITimezoneOffsetManager tzOffsetManager)
            : base(dbContext, tzOffsetManager)
        {
        }
    }
}
