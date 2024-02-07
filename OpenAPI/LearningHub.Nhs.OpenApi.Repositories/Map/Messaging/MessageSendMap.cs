// <copyright file="MessageSendMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Messaging
{
    using LearningHub.Nhs.Models.Entities.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The MessageSendMap class.
    /// </summary>
    public class MessageSendMap : BaseEntityMap<MessageSend>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<MessageSend> modelBuilder)
        {
            modelBuilder.ToTable("MessageSend", "messaging");

            modelBuilder.HasOne(x => x.Message)
                .WithMany(x => x.MessageSends)
                .HasForeignKey("MessageId")
                .HasConstraintName("FK_MessageSend_Message");

            modelBuilder.Property(e => e.Status)
                .IsRequired();

            modelBuilder.HasMany(x => x.Recipients)
                .WithOne(x => x.MessageSend)
                .HasForeignKey("MessageSendId")
                .HasConstraintName("FK_MessageSendRecipient_MessageSend");

            modelBuilder.HasOne(x => x.MessageType)
                .WithMany()
                .HasForeignKey(x => x.MessageTypeId)
                .HasConstraintName("FK_MessageSend_MessageType");
        }
    }
}
