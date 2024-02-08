namespace LearningHub.Nhs.Repository.Map.Content
{
    using LearningHub.Nhs.Models.Entities.Content;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="ImageAssetMap" />.
    /// </summary>
    public class ImageAssetMap : BaseEntityMap<ImageAsset>
    {
        /// <summary>
        /// The InternalMap.
        /// </summary>
        /// <param name="entity">The entity<see cref="EntityTypeBuilder{ImageAsset}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<ImageAsset> entity)
        {
            entity.ToTable("ImageAsset", "content");

            entity.Property(e => e.AltTag).HasMaxLength(125);

            entity.HasOne(d => d.ImageFile)
                .WithMany(p => p.ImageAssets)
                .HasForeignKey(d => d.ImageFileId)
                .HasConstraintName("FK_ImageAsset_File");
        }
    }
}
