namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The whole slide image annotation map.
    /// </summary>
    public class ImageAnnotationMap : BaseEntityMap<ImageAnnotation>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ImageAnnotation> modelBuilder)
        {
            modelBuilder.ToTable("ImageAnnotation", "resources");

            modelBuilder.HasOne(a => a.WholeSlideImage)
                .WithMany(i => i.ImageAnnotations)
                .HasForeignKey(a => a.WholeSlideImageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ImageAnnotation_WholeSlideImageId");
        }
    }
}
