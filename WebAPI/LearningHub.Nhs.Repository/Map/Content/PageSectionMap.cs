// <copyright file="PageSectionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Content
{
    using LearningHub.Nhs.Models.Entities.Content;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="PageSectionMap" />.
    /// </summary>
    public class PageSectionMap : BaseEntityMap<PageSection>
    {
        /// <summary>
        /// The InternalMap.
        /// </summary>
        /// <param name="entity">The entity<see cref="EntityTypeBuilder{PageSection}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<PageSection> entity)
        {
            entity.ToTable("PageSection", "content");

            entity.HasOne(d => d.Page)
                .WithMany(p => p.PageSections)
                .HasForeignKey(d => d.PageId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageSectionGroup_Page");
        }
    }
}
