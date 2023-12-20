// <copyright file="ScormActivityInteractionMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The scorm activity interaction map.
    /// </summary>
    public class ScormActivityInteractionMap : BaseEntityMap<ScormActivityInteraction>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ScormActivityInteraction> modelBuilder)
        {
            modelBuilder.ToTable("ScormActivityInteraction", "activity");

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.Property(e => e.InteractionId).HasMaxLength(255);

            modelBuilder.Property(e => e.Latency).HasMaxLength(255);

            modelBuilder.Property(e => e.Result).HasMaxLength(255);

            modelBuilder.Property(e => e.StudentResponse)
                .HasColumnName("Student_response")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.Type).HasMaxLength(255);

            modelBuilder.Property(e => e.Weighting).HasColumnType("decimal(16, 2)");

            modelBuilder.HasOne(d => d.ScormActivity)
                .WithMany(p => p.ScormActivityInteraction)
                .HasForeignKey(d => d.ScormActivityId)
                .HasConstraintName("FK_ScormActivityInteraction_ScormActivity");
        }
    }
}
