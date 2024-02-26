namespace LearningHub.Nhs.OpenApi.Repositories.Map.Messaging
{
    using LearningHub.Nhs.Models.Entities.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The MessageSendRecipientMap class.
    /// </summary>
    public class MessageTypeMap : BaseEntityMap<MessageType>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<MessageType> modelBuilder)
        {
            modelBuilder.ToTable("MessageType", "messaging");

            modelBuilder.Property(x => x.Type)
                .HasMaxLength(50)
                .IsRequired();
        }
    }
}
