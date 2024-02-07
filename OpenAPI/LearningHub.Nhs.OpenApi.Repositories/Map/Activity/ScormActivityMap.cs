// <copyright file="ScormActivityMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The scorm activity map.
    /// </summary>
    public class ScormActivityMap : BaseEntityMap<ScormActivity>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ScormActivity> modelBuilder)
        {
            modelBuilder.ToTable("ScormActivity", "activity");

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CmiCoreExit).HasMaxLength(255);

            modelBuilder.Property(e => e.CmiCoreLessonLocation)
                .HasColumnName("CmiCoreLesson_location")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.CmiCoreLessonStatus).HasColumnName("CmiCoreLesson_status");

            modelBuilder.Property(e => e.CmiCoreScoreRaw).HasColumnType("decimal(16, 2)");

            modelBuilder.Property(e => e.CmiCoreSessionTime)
                .HasColumnName("CmiCoreSession_time")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.CmiSuspendData).HasColumnName("CmiSuspend_data");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.Property(e => e.ResourceActivityId).HasColumnName("ResourceActivityId");

            modelBuilder.HasOne(d => d.ResourceActivity)
                .WithMany(p => p.ScormActivity)
                .HasForeignKey(d => d.ResourceActivityId)
                .HasConstraintName("FK_ScormActivity_ResourceActivity");
        }
    }
}
