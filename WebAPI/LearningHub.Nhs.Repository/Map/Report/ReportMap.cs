// <copyright file="ReportMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Report
{
    using LearningHub.Nhs.Models.Entities.Reporting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The report map.
    /// </summary>
    public class ReportMap : BaseEntityMap<Report>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Report> modelBuilder)
        {
            modelBuilder.ToTable("Report", "reports");

            modelBuilder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("Name")
                .HasMaxLength(128);

            modelBuilder.Property(e => e.Deleted).HasColumnName("Deleted");

            modelBuilder.Property(e => e.FileName)
                .IsRequired()
                .HasColumnName("FileName")
                .HasMaxLength(512);

            modelBuilder.Property(e => e.Hash)
                .IsRequired()
                .HasColumnName("Hash")
                .HasMaxLength(64);

            modelBuilder.Property(e => e.ClientId)
                .HasColumnName("ClientId")
                .HasConversion<int>()
                .IsRequired();

            modelBuilder.HasOne(e => e.Client)
             .WithMany()
             .HasForeignKey(x => x.ClientId)
             .HasConstraintName("FK_Report_Client");

            modelBuilder.Property(e => e.ReportStatusId)
                .HasColumnName("ReportStatusId")
                .HasConversion<int>()
                .IsRequired();

            modelBuilder.HasOne(e => e.ReportStatus)
              .WithMany()
              .HasForeignKey(x => x.ReportStatusId)
              .HasConstraintName("FK_Report_ReportStatus");

            modelBuilder.Property(e => e.ReportTypeId)
             .HasColumnName("ReportTypeId")
             .HasConversion<int>()
             .IsRequired();

            modelBuilder.HasOne(e => e.ReportType)
             .WithMany()
             .HasForeignKey(x => x.ReportTypeId)
             .HasConstraintName("FK_Report_ReportType");
        }
    }
}