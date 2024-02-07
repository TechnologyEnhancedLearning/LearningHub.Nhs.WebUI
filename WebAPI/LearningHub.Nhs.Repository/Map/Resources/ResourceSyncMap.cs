// <copyright file="ResourceSyncMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The ResourceSyncMap.
    /// </summary>
    public class ResourceSyncMap : BaseEntityMap<ResourceSync>
    {
        /// <summary>
        /// The InternalMap.
        /// </summary>
        /// <param name="builder">The builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceSync> builder)
        {
            builder.ToTable("ResourceSync", "resources");

            builder.Property(x => x.Id);

            builder.Property(x => x.ResourceId)
                .IsRequired(true);

            builder.Property(x => x.UserId)
                .IsRequired(true);

            builder.HasOne(x => x.Resource)
                .WithMany()
                .HasForeignKey(x => x.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceSync_ResourceVersion");
        }
    }
}
