// <copyright file="UserProfileMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The user profile map.
    /// </summary>
    public class UserProfileMap : BaseEntityMap<UserProfile>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<UserProfile> modelBuilder)
        {
            modelBuilder.ToTable("UserProfile", "hub");

            modelBuilder.Property(e => e.UserName)
                .HasColumnName("UserName")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.EmailAddress)
                .IsRequired()
                .HasColumnName("EmailAddress")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.FirstName)
                .IsRequired()
                .HasColumnName("FirstName")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.LastName)
                .IsRequired()
                .HasColumnName("LastName")
                .HasMaxLength(50);
        }
    }
}
