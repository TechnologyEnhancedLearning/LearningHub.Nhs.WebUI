// <copyright file="PermissionRoleMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The permission role map.
    /// </summary>
    public class PermissionRoleMap : BaseEntityMap<PermissionRole>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<PermissionRole> modelBuilder)
        {
            modelBuilder.ToTable("PermissionRole", "hub");

            modelBuilder.Property(e => e.PermissionId).HasColumnName("PermissionId");

            modelBuilder.Property(e => e.RoleId).HasColumnName("RoleId");

            modelBuilder.HasOne(d => d.Permission)
                .WithMany(p => p.PermissionRole)
                .HasForeignKey(d => d.PermissionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_permissionRole_permission");

            modelBuilder.HasOne(d => d.Role)
                .WithMany(p => p.PermissionRole)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_userPermissionRole_role");
        }
    }
}
