namespace LearningHub.Nhs.Repository
{
    using LearningHub.Nhs.Repository.Interface;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// The TransactionManager.
    /// </summary>
    public class TransactionManager : ITransactionManager
    {
        private readonly LearningHubDbContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionManager"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public TransactionManager(LearningHubDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Begins a transaction, for use in a using block.
        /// </summary>
        /// <returns>A DbContextTransaction.</returns>
        public IDbContextTransaction BeginTransaction()
        {
            return this.context.Database.BeginTransaction();
        }
    }
}
