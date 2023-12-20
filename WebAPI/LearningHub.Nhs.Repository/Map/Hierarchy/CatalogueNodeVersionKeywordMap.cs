// <copyright file="CatalogueNodeVersionKeywordMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The CatalogueNodeVersionKeywordMap.
    /// </summary>
    public class CatalogueNodeVersionKeywordMap : BaseEntityMap<CatalogueNodeVersionKeyword>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder.</param>
        protected override void InternalMap(EntityTypeBuilder<CatalogueNodeVersionKeyword> modelBuilder)
        {
            modelBuilder.ToTable("CatalogueNodeVersionKeyword", "hierarchy");

            modelBuilder.Property(x => x.CatalogueNodeVersionId)
                .IsRequired();

            modelBuilder.Property(x => x.Keyword)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
