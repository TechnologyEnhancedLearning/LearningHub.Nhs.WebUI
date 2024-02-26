namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The image annotation mark map.
    /// </summary>
    public class ImageAnnotationMarkMap : BaseEntityMap<ImageAnnotationMark>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ImageAnnotationMark> modelBuilder)
        {
            modelBuilder.ToTable("ImageAnnotationMark", "resources");

            modelBuilder.HasOne(a => a.ImageAnnotation)
                .WithMany(i => i.ImageAnnotationMarks)
                .HasForeignKey(a => a.ImageAnnotationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ImageAnnotationMark_ImageAnnotationId");
        }
    }
}
