// <copyright file="ResourceVersionKeywordMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource version keyword map.
    /// </summary>
    public class ResourceVersionKeywordMap : BaseEntityMap<ResourceVersionKeyword>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionKeyword> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersionKeyword", "resources");

            modelBuilder.Property(e => e.Keyword).HasMaxLength(50);

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithMany(p => p.ResourceVersionKeyword)
                .HasForeignKey(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceVersionAuthor_ResourceVersion");
        }
    }
}
