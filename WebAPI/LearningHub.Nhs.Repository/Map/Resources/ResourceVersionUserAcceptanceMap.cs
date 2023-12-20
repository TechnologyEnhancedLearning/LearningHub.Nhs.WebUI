// <copyright file="ResourceVersionUserAcceptanceMap.cs" company="HEE.nhs.uk">
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
    public class ResourceVersionUserAcceptanceMap : BaseEntityMap<ResourceVersionUserAcceptance>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionUserAcceptance> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersionUserAcceptance", "resources");

            modelBuilder.HasOne(d => d.ResourceVersion)
                        .WithMany(p => p.ResourceVersionUserAcceptance)
                        .HasForeignKey(d => d.ResourceVersionId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ResourceVersionUserAcceptance_ResourceVersion");
        }
    }
}
