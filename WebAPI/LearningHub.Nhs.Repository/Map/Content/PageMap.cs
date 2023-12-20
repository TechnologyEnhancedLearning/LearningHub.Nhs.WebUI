// <copyright file="PageMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Content
{
    using LearningHub.Nhs.Models.Entities.Content;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="PageMap" />.
    /// </summary>
    public class PageMap : BaseEntityMap<Page>
    {
        /// <summary>
        /// The InternalMap.
        /// </summary>
        /// <param name="entity">The entity<see cref="EntityTypeBuilder{Page}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<Page> entity)
        {
            entity.ToTable("Page", "content");

            entity.HasIndex(e => e.Name)
                .IsUnique();

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(128);
        }
    }
}
