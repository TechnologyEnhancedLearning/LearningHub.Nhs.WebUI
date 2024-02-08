namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Scorm resource version map.
    /// </summary>
    public class ExternalReferenceMap : BaseEntityMap<ExternalReference>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ExternalReference> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_ExternalReference");

            modelBuilder.ToTable("ExternalReference", "resources");

            modelBuilder.Property(t => t.Reference).HasColumnName("ExternalReference");
        }
    }
}
