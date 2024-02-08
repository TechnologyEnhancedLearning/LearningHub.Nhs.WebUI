namespace LearningHub.Nhs.Repository
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Repository.Map;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The learning hub db context options.
    /// </summary>
    public class LearningHubDbContextOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LearningHubDbContextOptions"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="mappings">The mappings.</param>
        public LearningHubDbContextOptions(DbContextOptions<LearningHubDbContext> options, IEnumerable<IEntityTypeMap> mappings)
        {
            this.Options = options;
            this.Mappings = mappings;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public DbContextOptions<LearningHubDbContext> Options { get; }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        public IEnumerable<IEntityTypeMap> Mappings { get; }
    }
}
