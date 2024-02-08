namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Analytics;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The event map.
    /// </summary>
    public class EventMap : BaseEntityMap<Event>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Event> modelBuilder)
        {
            modelBuilder.ToTable("Event", "analytics");

            modelBuilder.Property(e => e.UserId)
                .IsRequired()
                .HasColumnName("UserId")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.EventTypeEnum).HasColumnName("EventTypeId")
              .HasConversion<int>();

            modelBuilder.Property(e => e.JsonData)
             .HasColumnName("JsonData");
        }
    }
}
