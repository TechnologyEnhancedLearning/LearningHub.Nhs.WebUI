namespace LearningHub.Nhs.Repository.Map.Report
{
    using LearningHub.Nhs.Models.Entities.Reporting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The ReportTypeMap class.
    /// </summary>
    public class ReportTypeMap : BaseEntityMap<ReportType>
    {
        /// <summary>
        /// The report type map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ReportType> modelBuilder)
        {
            modelBuilder.ToTable("ReportType", "reports");

            modelBuilder.Property(x => x.Name)
                .HasMaxLength(128)
                .IsRequired();

            modelBuilder.Property(x => x.Description)
               .HasMaxLength(512)
               .IsRequired();
        }
    }
}
