// <copyright file="EquipmentResourceVersionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The equipment resource version map.
    /// </summary>
    public class EquipmentResourceVersionMap : BaseEntityMap<EquipmentResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<EquipmentResourceVersion> modelBuilder)
        {
            modelBuilder.ToTable("EquipmentResourceVersion", "resources");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.Property(e => e.ContactEmail).HasMaxLength(255);

            modelBuilder.Property(e => e.ContactName).HasMaxLength(255);

            modelBuilder.Property(e => e.ContactTelephone).HasMaxLength(255);

            modelBuilder.HasOne(d => d.Address)
                .WithMany(p => p.EquipmentResourceVersion)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK_EquipmentResourceVersion_Address");

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithOne(p => p.EquipmentResourceVersion)
                .HasForeignKey<EquipmentResourceVersion>(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_EquipmentResourceVersion_ResourceVersion");
        }
    }
}
