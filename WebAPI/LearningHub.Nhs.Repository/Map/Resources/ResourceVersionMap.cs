// <copyright file="ResourceVersionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource version map.
    /// </summary>
    public class ResourceVersionMap : BaseEntityMap<ResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersion> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersion", "resources");

            modelBuilder.Property(e => e.AdditionalInformation)
                .IsRequired()
                .HasMaxLength(250);

            modelBuilder.Property(e => e.Cost).HasColumnType("decimal(8, 2)");

            modelBuilder.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(1024);

            modelBuilder.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(255);

            modelBuilder.HasOne(d => d.Resource)
                    .WithMany(p => p.ResourceVersion)
                    .HasForeignKey(d => d.ResourceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ResourceVersion_Resource");

            modelBuilder.Property(e => e.ResourceAccessibilityEnum).HasColumnName("ResourceAccessibilityId")
              .HasConversion<int>();

            modelBuilder.Property(e => e.VersionStatusEnum).HasColumnName("VersionStatusId")
               .HasConversion<int>();

            modelBuilder.HasOne(d => d.ResourceLicence)
                .WithMany(p => p.ResourceVersion)
                .HasForeignKey(d => d.ResourceLicenceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceVersion_ResourceLicence");

            modelBuilder.HasOne(d => d.CreateUser)
                .WithMany(p => p.ResourceVersion)
                .HasForeignKey(d => d.CreateUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceVersion_CreateUser");
        }
    }
}
