namespace LearningHub.Nhs.OpenApi.Repositories.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The CatalogueNodeVersionCategoryMap.
    /// </summary>
    public class CatalogueNodeVersionCategoryMap : BaseEntityMap<CatalogueNodeVersionCategory>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder.</param>
        protected override void InternalMap(EntityTypeBuilder<CatalogueNodeVersionCategory> modelBuilder)
        {
            modelBuilder.ToTable("CatalogueNodeVersionCategory", "hierarchy");

            modelBuilder.Property(x => x.CatalogueNodeVersionId)
                .IsRequired();
            modelBuilder.Property(x => x.CategoryId)
                .IsRequired();
        }
    }
}
