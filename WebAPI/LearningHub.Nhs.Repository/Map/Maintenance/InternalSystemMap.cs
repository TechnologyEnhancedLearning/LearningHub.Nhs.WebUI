// <copyright file="InternalSystemMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Content
{
    using LearningHub.Nhs.Models.Entities.Maintenance;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="InternalSystemMap" />.
    /// </summary>
    public class InternalSystemMap : BaseEntityMap<InternalSystem>
    {
        /// <summary>
        /// The InternalMap.
        /// </summary>
        /// <param name="entity">The entity<see cref="EntityTypeBuilder{InternalSystem}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<InternalSystem> entity)
        {
            entity.ToTable("InternalSystem", "maintenance");

            entity.HasIndex(e => e.Name)
                .IsUnique();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(2000);
        }
    }
}
