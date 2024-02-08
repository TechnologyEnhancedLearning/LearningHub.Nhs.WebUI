namespace LearningHub.Nhs.Repository.Map.Report
{
    using LearningHub.Nhs.Models.Entities.Reporting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The ReportStatusMap class.
    /// </summary>
    public class ReportStatusMap : BaseEntityMap<ReportStatus>
    {
        /// <summary>
        /// The report status map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ReportStatus> modelBuilder)
        {
            modelBuilder.ToTable("ReportStatus", "reports");

            modelBuilder.Property(x => x.Name)
                .HasMaxLength(128)
                .IsRequired();

            modelBuilder.Property(x => x.Description)
               .HasMaxLength(512)
               .IsRequired();
        }
    }
}
