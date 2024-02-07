// <copyright file="MigrationStagingDbContext.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Migration.Staging.Repository
{
    using LearningHub.Nhs.Migration.Models;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The migration tool staging tables db context. This is used as a source of migration input data. They are
    /// populated by an Azure Data Factory pipeline created by Jeremy. It is a separate db to the LH one.
    /// </summary>
    public partial class MigrationStagingDbContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationStagingDbContext"/> class.
        /// </summary>
        /// <param name="options">The options.</param>
        public MigrationStagingDbContext(DbContextOptions<MigrationStagingDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the StagingTableInputModels. These are not entities. They are returned from the [Migration].[GetStagingTableResources]
        /// stored proc in the migration staging table database.
        /// </summary>
        public virtual DbSet<StagingTableInputModel> StagingTableInputModels { get; set; }
    }
}
