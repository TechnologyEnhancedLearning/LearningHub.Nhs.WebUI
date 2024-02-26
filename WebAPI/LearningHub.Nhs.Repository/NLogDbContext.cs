namespace LearningHub.Nhs.Repository
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The nlog db context.
    /// </summary>
    public partial class NLogDbContext : DbContext
    {
        /// <summary>
        /// The options.
        /// </summary>
        private readonly NLogDbContextOptions options;

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogDbContext"/> class.
        /// </summary>
        /// <param name="options">The options<see cref="NLogDbContextOptions"/>.</param>
        public NLogDbContext(NLogDbContextOptions options)
            : base(options.Options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets or sets the Log.
        /// </summary>
        public virtual DbSet<Log> Log { get; set; }

        /// <summary>
        /// The on model creating.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder<see cref="ModelBuilder"/>.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var mapping in this.options.Mappings)
            {
                mapping.Map(modelBuilder);
            }
        }
    }
}
