namespace LearningHub.Nhs.Repository
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Repository.Map;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// DLSDbContextOptions class.
    /// </summary>
    public class DLSDbContextOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DLSDbContextOptions"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <param name="mappings">The mappings.</param>
        public DLSDbContextOptions(DbContextOptions<DLSDbContext> options, IEnumerable<IEntityTypeMap> mappings)
        {
            this.Options = options;
            this.Mappings = mappings;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public DbContextOptions<DLSDbContext> Options { get; }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        public IEnumerable<IEntityTypeMap> Mappings { get; }
    }
}
