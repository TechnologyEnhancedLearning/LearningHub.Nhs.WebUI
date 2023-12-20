// <copyright file="MediaResourceActivityMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The media resource activity event map.
    /// </summary>
    public class MediaResourceActivityMap : BaseEntityMap<MediaResourceActivity>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<MediaResourceActivity> modelBuilder)
        {
            modelBuilder.ToTable("MediaResourceActivity", "activity");

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.HasOne(d => d.ResourceActivity)
                .WithMany(p => p.MediaResourceActivity)
                .HasForeignKey(d => d.ResourceActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MediaResourceActivity_ResourceActivity");
        }
    }
}
