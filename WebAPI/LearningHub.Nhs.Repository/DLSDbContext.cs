namespace LearningHub.Nhs.Repository
{
    using LearningHub.Nhs.Api.DLSEntities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The DLS db context.
    /// </summary>
    public class DLSDbContext : DbContext
    {
        /// <summary>
        /// The options..
        /// </summary>
        private readonly DbContextOptions<DLSDbContext> options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DLSDbContext"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="DbContextOptions"/>.</param>
        public DLSDbContext(DbContextOptions<DLSDbContext> options)
            : base(options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets or sets the dataset for the DLS device type.
        /// </summary>
        public DbSet<DeviceTypes> DeviceTypes { get; set; }
    }
}
