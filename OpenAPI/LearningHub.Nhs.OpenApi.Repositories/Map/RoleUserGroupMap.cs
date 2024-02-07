// <copyright file="RoleUserGroupMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The role user group map.
    /// </summary>
    public class RoleUserGroupMap : BaseEntityMap<RoleUserGroup>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<RoleUserGroup> modelBuilder)
        {
            modelBuilder.ToTable("RoleUserGroup", "hub");

            modelBuilder.Property(e => e.RoleId).HasColumnName("RoleId");

            modelBuilder.Property(e => e.UserGroupId).HasColumnName("UserGroupId");

            modelBuilder.HasOne(d => d.Role)
                .WithMany(p => p.RoleUserGroup)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_userRoleUserGroup_userRole");

            modelBuilder.HasOne(d => d.Scope)
                .WithMany()
                .HasForeignKey(d => d.ScopeId)
                .HasConstraintName("FK_userRoleUserGroup_scope");

            modelBuilder.HasOne(d => d.UserGroup)
                .WithMany(p => p.RoleUserGroup)
                .HasForeignKey(d => d.UserGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_userRoleUserGroup_userGroup");
        }
    }
}
