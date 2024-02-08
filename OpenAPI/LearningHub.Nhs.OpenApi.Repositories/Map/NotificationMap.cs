namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The notification map.
    /// </summary>
    public class NotificationMap : BaseEntityMap<Notification>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Notification> modelBuilder)
        {
            modelBuilder.ToTable("Notification", "hub");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.Id)
                    .HasColumnName("Id");

            modelBuilder.Property(e => e.Title)
                .IsRequired()
                .HasColumnName("Title")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.Deleted).HasColumnName("Deleted");

            modelBuilder.Property(e => e.Message)
                .HasColumnName("Message");

            modelBuilder.Property(e => e.StartDate)
                .HasColumnName("StartDate")
                .IsRequired();

            modelBuilder.Property(e => e.EndDate)
                .HasColumnName("EndDate")
                .IsRequired();

            modelBuilder.Property(e => e.UserDismissable)
                .HasColumnName("UserDismissable")
                .IsRequired();

            modelBuilder.Property(e => e.IsUserSpecific)
                .HasColumnName("IsUserSpecific")
                .IsRequired();

            modelBuilder.HasOne(d => d.CreateUser)
                .WithMany(p => p.CreatedNotifications)
                .HasForeignKey(d => d.CreateUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_userCreated");

            modelBuilder.HasOne(d => d.AmendUser)
                .WithMany(p => p.AmendedNotifications)
                .HasForeignKey(d => d.AmendUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Notification_userAmended");

            modelBuilder.Property(e => e.NotificationTypeEnum)
                .HasColumnName("NotificationTypeId")
                .HasConversion<int>()
                .IsRequired();

            modelBuilder.Property(e => e.NotificationPriorityEnum)
                .HasColumnName("NotificationPriorityId")
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
