// <copyright file="NotificationPriorityMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The user type map.
    /// </summary>
    public class NotificationPriorityMap : BaseEntityMap<NotificationPriority>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<NotificationPriority> modelBuilder)
        {
            modelBuilder.ToTable("NotificationPriority", "hub");

            modelBuilder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("Name")
                .HasMaxLength(100)
                .IsUnicode(false);

            modelBuilder.Property(e => e.Description)
                .HasColumnName("Description")
                .HasMaxLength(255)
                .IsUnicode(false);
        }
    }
}
