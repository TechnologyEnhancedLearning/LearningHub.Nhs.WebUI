// <copyright file="MessageMetaDataMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Messaging
{
    using LearningHub.Nhs.Models.Entities.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The MessageMetaDataMap class.
    /// </summary>
    public class MessageMetaDataMap : BaseEntityMap<MessageMetaData>, IEntityTypeMap
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<MessageMetaData> modelBuilder)
        {
            modelBuilder.ToTable("MessageMetaData", "messaging");

            modelBuilder.Property(e => e.NotificationEndDate)
                .IsRequired();

            modelBuilder.Property(e => e.NotificationStartDate)
                .IsRequired();

            modelBuilder.Property(e => e.NotificationPriority)
                .IsRequired();

            modelBuilder.Property(e => e.NotificationType)
                .IsRequired();

            modelBuilder.HasOne(x => x.Message)
                .WithOne(x => x.MessageMetaData)
                .HasForeignKey("MessageMetaData", "MessageId")
                .HasConstraintName("FK_MessageMetaData_Message");
        }
    }
}
