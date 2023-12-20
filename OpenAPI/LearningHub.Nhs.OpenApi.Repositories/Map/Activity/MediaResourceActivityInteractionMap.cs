// <copyright file="MediaResourceActivityInteractionMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource activity map.
    /// </summary>
    public class MediaResourceActivityInteractionMap : BaseEntityMap<MediaResourceActivityInteraction>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<MediaResourceActivityInteraction> modelBuilder)
        {
            modelBuilder.ToTable("MediaResourceActivityInteraction", "activity");

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.HasOne(d => d.MediaResourceActivityType)
                    .WithMany(p => p.MediaResourceActivityInteraction)
                    .HasForeignKey(d => d.MediaResourceActivityTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_MediaResourceActivity_MediaResourceActivityType");
        }
    }
}
