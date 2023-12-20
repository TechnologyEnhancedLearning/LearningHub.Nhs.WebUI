// <copyright file="ResourceSyncMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using System;
    using System.Collections.Generic;
    using System.Text;
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
