// <copyright file="ResourceLicenceMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource licence map.
    /// </summary>
    public class ResourceLicenceMap : BaseEntityMap<ResourceLicence>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceLicence> modelBuilder)
        {
            modelBuilder.ToTable("ResourceLicence", "resources");

            modelBuilder.Property(e => e.Id).ValueGeneratedNever();

            modelBuilder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(128);
        }
    }
}
