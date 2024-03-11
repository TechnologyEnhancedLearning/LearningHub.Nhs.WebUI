namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The image map.
    /// </summary>
    public class ImageMap : BaseEntityMap<Image>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Image> modelBuilder)
        {
            modelBuilder.ToTable("Image", "resources");

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.Images)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Image_FileId");
        }
    }
}