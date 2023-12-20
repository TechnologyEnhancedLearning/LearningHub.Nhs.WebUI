// <copyright file="ResourceVersionRatingMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource version rating summary map.
    /// </summary>
    public class ResourceVersionRatingMap : BaseEntityMap<ResourceVersionRating>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionRating> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersionRating", "resources");

            modelBuilder.HasOne(d => d.ResourceVersion)
                        .WithMany(p => p.ResourceVersionRatings)
                        .HasForeignKey(d => d.ResourceVersionId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Rating_ResourceVersion");

            // modelBuilder.HasOne(d => d.User)
            //    .WithMany(p => p.ResourceVersionRating)
            //    .HasForeignKey(d => d.CreateUserId)
            //    .OnDelete(DeleteBehavior.ClientSetNull)
            //    .HasConstraintName("FK_ResourceVersionEvent_CreateUser");
        }
    }
}
