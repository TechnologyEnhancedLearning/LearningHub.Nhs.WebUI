namespace LearningHub.Nhs.OpenApi.Repositories.Map.Messaging
{
    using LearningHub.Nhs.Models.Entities.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The MessageMap class.
    /// </summary>
    public class MessageMap : BaseEntityMap<Message>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Message> modelBuilder)
        {
            modelBuilder.ToTable("Message", "messaging");

            modelBuilder.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(512);

            modelBuilder.Property(e => e.Body)
                .IsRequired();

            modelBuilder.HasMany(x => x.MessageSends)
                .WithOne(x => x.Message)
                .HasForeignKey("MessageId")
                .HasConstraintName("FK_MessageSend_Message");
        }
    }
}
