// <copyright file="ScormActivityInteractionCorrectResponseMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The scorm activity interaction correct response map.
    /// </summary>
    public class ScormActivityInteractionCorrectResponseMap : BaseEntityMap<ScormActivityInteractionCorrectResponse>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ScormActivityInteractionCorrectResponse> modelBuilder)
        {
            modelBuilder.ToTable("ScormActivityInteractionCorrectResponse", "activity");

            modelBuilder.Property(e => e.Pattern).HasMaxLength(255);

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.HasOne(d => d.ScormActivityInteraction)
                .WithMany(p => p.ScormActivityInteractionCorrectResponse)
                .HasForeignKey(d => d.ScormActivityInteractionId)
                .HasConstraintName("FK_ScormActivityInteractionCorrectResponse_ScormActivityInteraction");
        }
    }
}
