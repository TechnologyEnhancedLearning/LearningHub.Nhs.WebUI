namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// the image carousel block map.
    /// </summary>
    public class ImageCarouselBlockMap : BaseEntityMap<ImageCarouselBlock>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ImageCarouselBlock> modelBuilder)
        {
            modelBuilder.ToTable("ImageCarouselBlock", "resources");

            modelBuilder.Property(e => e.BlockId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.BlockId);

            modelBuilder.HasOne(d => d.Block)
                .WithOne(p => p.ImageCarouselBlock)
                .HasForeignKey<ImageCarouselBlock>(d => d.BlockId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ImageCarouselBlock_BlockId");

            modelBuilder.HasOne(d => d.ImageBlockCollection)
                .WithOne()
                .HasForeignKey<ImageCarouselBlock>(d => d.ImageBlockCollectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ImageCarouselBlock_ImageBlockCollectionId");
        }
    }
}