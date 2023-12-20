// <copyright file="ScormActivityObjectiveMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The scorm activity objective map.
    /// </summary>
    public class ScormActivityObjectiveMap : BaseEntityMap<ScormActivityObjective>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ScormActivityObjective> modelBuilder)
        {
            modelBuilder.ToTable("ScormActivityObjective", "activity");

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.Property(e => e.ObjectiveId).HasMaxLength(255);

            modelBuilder.Property(e => e.ScoreMax)
                .HasColumnName("Score_max")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.ScoreMin)
                .HasColumnName("Score_min")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.ScoreRaw)
                .HasColumnName("Score_raw")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.Status).HasMaxLength(255);

            modelBuilder.HasOne(d => d.ScormActivity)
                .WithMany(p => p.ScormActivityObjective)
                .HasForeignKey(d => d.ScormActivityId)
                .HasConstraintName("FK_ScormActivityObjective_ScormActivity");
        }
    }
}
