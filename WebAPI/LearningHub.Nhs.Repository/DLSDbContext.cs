namespace LearningHub.Nhs.Repository
{
    using LearningHub.Nhs.Models.DLS;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The DLS db context.
    /// </summary>
    public class DLSDbContext : DbContext
    {
        /// <summary>
        /// The options..
        /// </summary>
        private readonly DLSDbContextOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="DLSDbContext"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="DbContextOptions"/>.</param>
        public DLSDbContext(DLSDbContextOptions options)
            : base(options.Options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets the Options.
        /// </summary>
        public DLSDbContextOptions Options
        {
            get { return this.options; }
        }

        /// <summary>
        /// Gets or sets the dataset for the DLS device type.
        /// </summary>
        public DbSet<DeviceTypes> DeviceTypes { get; set; }
    }
}
