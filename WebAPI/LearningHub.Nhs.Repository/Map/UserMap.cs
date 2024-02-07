// <copyright file="UserMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The user map.
    /// </summary>
    public class UserMap : BaseEntityMap<User>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<User> modelBuilder)
        {
            modelBuilder.ToTable("User", "hub");

            modelBuilder.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            modelBuilder.Property(e => e.UserName)
                .IsRequired()
                .HasColumnName("UserName")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.VersionEndTime)
                .HasColumnName("VersionEndTime")
                .HasDefaultValueSql("(CONVERT([datetime2],'9999-12-31 23:59:59.9999999'))");

            modelBuilder.Property(e => e.VersionStartTime)
                .HasColumnName("VersionStartTime")
                .HasDefaultValueSql("(getutcdate())");

            modelBuilder.Ignore(e => e.Token);
            modelBuilder.Ignore(e => e.AssignedRoles);
        }
    }
}
