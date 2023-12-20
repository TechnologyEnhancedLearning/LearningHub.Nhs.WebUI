// <copyright file="MessageSendRecipientMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Messaging
{
    using LearningHub.Nhs.Models.Entities.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The MessageSendRecipientMap class.
    /// </summary>
    public class MessageSendRecipientMap : BaseEntityMap<MessageSendRecipient>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<MessageSendRecipient> modelBuilder)
        {
            modelBuilder.ToTable("MessageSendRecipient", "messaging");

            modelBuilder.Property(x => x.UserId)
                .IsRequired(false);

            modelBuilder.Property(x => x.UserGroupId)
                .IsRequired(false);

            modelBuilder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey("UserId")
                .IsRequired(false);

            modelBuilder.HasOne(x => x.UserGroup)
                .WithMany()
                .HasForeignKey("UserGroupId")
                .IsRequired(false);

            modelBuilder.HasOne(x => x.MessageSend)
                .WithMany(x => x.Recipients)
                .HasForeignKey("MessageSendId")
                .HasConstraintName("FK_MessageSendRecipient_MessageSend");
        }
    }
}
