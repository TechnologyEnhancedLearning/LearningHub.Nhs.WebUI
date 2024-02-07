// <copyright file="ResourceTypeMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.Map;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// A map.
    /// </summary>
    public class ResourceTypeMap : BaseEntityMap<ResourceType>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ResourceType> modelBuilder)
        {
            modelBuilder.ToTable("ResourceType", "resources");

            modelBuilder.Property(e => e.Id);

            modelBuilder.Property(e => e.Name).IsRequired().HasMaxLength(128);
        }
    }
}
