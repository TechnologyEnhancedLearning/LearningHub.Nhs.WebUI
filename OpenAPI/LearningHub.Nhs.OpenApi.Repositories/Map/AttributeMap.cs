namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Attribute map.
    /// </summary>
    public class AttributeMap : BaseEntityMap<Attribute>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Attribute> modelBuilder)
        {
            modelBuilder.ToTable("Attribute", "hub");

            modelBuilder.Property(e => e.AttributeTypeEnum).HasColumnName("AttributeTypeId")
               .HasConversion<int>();
        }
    }
}
