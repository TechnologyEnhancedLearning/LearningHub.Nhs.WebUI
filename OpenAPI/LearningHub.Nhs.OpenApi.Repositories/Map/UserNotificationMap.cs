// <copyright file="UserNotificationMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The user notification map.
    /// </summary>
    public class UserNotificationMap : BaseEntityMap<UserNotification>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<UserNotification> modelBuilder)
        {
            modelBuilder.ToTable("UserNotification", "hub");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.Id)
                    .HasColumnName("Id");

            modelBuilder.Property(e => e.ReadOnDate)
                    .HasColumnName("ReadOnDate");

            modelBuilder.HasOne(d => d.AmendUser)
                     .WithMany(p => p.UserNotificationAmendUser)
                     .HasForeignKey(d => d.AmendUserId)
                     .OnDelete(DeleteBehavior.ClientSetNull)
                     .HasConstraintName("FK_UserNotification_userAmended");

            modelBuilder.HasOne(d => d.CreateUser)
                .WithMany(p => p.UserNotificationCreateUser)
                .HasForeignKey(d => d.CreateUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserNotification_userCreated");

            modelBuilder.HasOne(d => d.Notification)
                .WithMany(p => p.UserNotification)
                .HasForeignKey(d => d.NotificationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserNotification_notification");

            modelBuilder.HasOne(d => d.User)
                .WithMany(p => p.UserNotificationUser)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserNotification_user");
        }
    }
}
