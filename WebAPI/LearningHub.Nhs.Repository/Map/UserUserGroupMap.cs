// <copyright file="UserUserGroupMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The user user group map.
    /// </summary>
    public class UserUserGroupMap : BaseEntityMap<UserUserGroup>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<UserUserGroup> modelBuilder)
        {
            modelBuilder.ToTable("UserUserGroup", "hub");

            modelBuilder.Property(e => e.UserGroupId).HasColumnName("UserGroupId");

            modelBuilder.Property(e => e.UserId).HasColumnName("UserId");

            modelBuilder.HasOne(d => d.UserGroup)
                .WithMany(p => p.UserUserGroup)
                .HasForeignKey(d => d.UserGroupId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_userUserGroup_userGroup");

            modelBuilder.HasOne(d => d.User)
                .WithMany(p => p.UserUserGroup)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_userUserGroup_user");
        }
    }
}
