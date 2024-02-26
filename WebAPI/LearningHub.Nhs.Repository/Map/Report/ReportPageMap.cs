namespace LearningHub.Nhs.Repository.Map.Report
{
    using LearningHub.Nhs.Models.Entities.Reporting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The report page map.
    /// </summary>
    public class ReportPageMap : BaseEntityMap<ReportPage>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ReportPage> modelBuilder)
        {
            modelBuilder.ToTable("ReportPage", "reports");

            modelBuilder.Property(e => e.Html)
                .HasColumnName("Html")
                .IsRequired();

            modelBuilder.Property(e => e.ReportOrientationModeId)
                .HasColumnName("ReportOrientationModeId")
                .HasConversion<int>()
                .IsRequired();

            modelBuilder.HasOne(x => x.ReportOrientationMode)
              .WithMany()
              .HasForeignKey(x => x.ReportOrientationModeId)
              .HasConstraintName("FK_ReportPage_ReportOrientationMode");

            modelBuilder.HasOne(d => d.Report)
                      .WithMany(p => p.ReportPages)
                      .HasForeignKey(d => d.ReportId)
                      .OnDelete(DeleteBehavior.ClientSetNull)
                      .HasConstraintName("FK_ReportPage_Report");
        }
    }
}