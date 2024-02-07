// <copyright file="PermissionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The permission map.
    /// </summary>
    public class PermissionMap : BaseEntityMap<Permission>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Permission> modelBuilder)
        {
            modelBuilder.ToTable("Permission", "hub");

            modelBuilder.Property(e => e.Code)
                .HasColumnName("Code")
                .HasMaxLength(100)
                .IsUnicode(false);

            modelBuilder.Property(e => e.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsUnicode(false);

            modelBuilder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsUnicode(false);
        }
    }
}
