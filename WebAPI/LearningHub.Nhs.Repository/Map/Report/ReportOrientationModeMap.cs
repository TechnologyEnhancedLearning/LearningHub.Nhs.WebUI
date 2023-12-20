// <copyright file="ReportOrientationModeMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Report
{
    using LearningHub.Nhs.Models.Entities.Reporting;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The ReportOrientationModeMap class.
    /// </summary>
    public class ReportOrientationModeMap : BaseEntityMap<ReportOrientationMode>
    {
        /// <summary>
        /// The report orientation map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ReportOrientationMode> modelBuilder)
        {
            modelBuilder.ToTable("ReportOrientationModee", "reports");

            modelBuilder.Property(x => x.Name)
                .HasMaxLength(20)
                .IsRequired();
        }
    }
}
