// <copyright file="ProviderMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The provider map.
    /// </summary>
    public class ProviderMap : BaseEntityMap<Provider>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Provider> modelBuilder)
        {
            modelBuilder.ToTable("Provider", "hub");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.Id)
                    .HasColumnName("Id");

            modelBuilder.Property(e => e.Description)
                .HasColumnName("Description")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.Logo)
                .HasColumnName("Logo")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("Name")
                .HasMaxLength(255);
        }
    }
}
