// <copyright file="AddressMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The address map.
    /// </summary>
    public class AddressMap : BaseEntityMap<Address>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Address> modelBuilder)
        {
            modelBuilder.ToTable("Address", "hub");

            modelBuilder.Property(e => e.Address1).HasMaxLength(100);

            modelBuilder.Property(e => e.Address2).HasMaxLength(100);

            modelBuilder.Property(e => e.Address3).HasMaxLength(100);

            modelBuilder.Property(e => e.Address4).HasMaxLength(100);

            modelBuilder.Property(e => e.County).HasMaxLength(100);

            modelBuilder.Property(e => e.PostCode).HasMaxLength(8);

            modelBuilder.Property(e => e.Town).HasMaxLength(100);
        }
    }
}
