namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The block collection map.
    /// </summary>
    public class BlockCollectionMap : BaseEntityMap<BlockCollection>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<BlockCollection> modelBuilder)
        {
            modelBuilder.ToTable("BlockCollection", "resources");
        }
    }
}
