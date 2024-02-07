// <copyright file="ResourceAzureMediaAssetMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resourceAzureMediaAsset map.
    /// </summary>
    public class ResourceAzureMediaAssetMap : BaseEntityMap<ResourceAzureMediaAsset>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceAzureMediaAsset> modelBuilder)
        {
            modelBuilder.ToTable("ResourceAzureMediaAsset", "resources");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.EncodeJobName)
                .IsRequired()
                .HasMaxLength(128);

            modelBuilder.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(1024);

            modelBuilder.Property(e => e.LocatorUri)
                .IsRequired()
                .HasMaxLength(1024);
        }
    }
}
