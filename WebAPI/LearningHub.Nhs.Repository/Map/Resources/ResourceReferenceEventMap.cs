namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="ResourceReferenceEventMap" />.
    /// </summary>
    public class ResourceReferenceEventMap : BaseEntityMap<ResourceReferenceEvent>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder<see cref="EntityTypeBuilder{ResourceReferenceEvent}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceReferenceEvent> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_ResourceReferenceEvent");

            modelBuilder.ToTable("ResourceReferenceEvent", "resources");
        }
    }
}
