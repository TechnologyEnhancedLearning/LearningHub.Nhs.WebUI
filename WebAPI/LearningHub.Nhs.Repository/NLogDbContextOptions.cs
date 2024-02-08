namespace LearningHub.Nhs.Repository
{
    using System.Collections.Generic;
    using LearningHub.Nhs.Repository.Map;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The nlog db context options.
    /// </summary>
    public class NLogDbContextOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NLogDbContextOptions"/> class.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="mappings">
        /// The mappings.
        /// </param>
        public NLogDbContextOptions(DbContextOptions<NLogDbContext> options, IEnumerable<NLogMap.IEntityTypeMap> mappings)
        {
            this.Options = options;
            this.Mappings = mappings;
        }

        /// <summary>
        /// Gets the options.
        /// </summary>
        public DbContextOptions<NLogDbContext> Options { get; }

        /// <summary>
        /// Gets the mappings.
        /// </summary>
        public IEnumerable<NLogMap.IEntityTypeMap> Mappings { get; }
    }
}
