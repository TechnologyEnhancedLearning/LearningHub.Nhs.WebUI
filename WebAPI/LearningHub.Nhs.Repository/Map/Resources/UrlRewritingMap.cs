// <copyright file="UrlRewritingMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Scorm resource version map.
    /// </summary>
    public class UrlRewritingMap : BaseEntityMap<UrlRewriting>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<UrlRewriting> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_UrlRewriting");

            modelBuilder.ToTable("UrlRewriting", "resources");
        }
    }
}
