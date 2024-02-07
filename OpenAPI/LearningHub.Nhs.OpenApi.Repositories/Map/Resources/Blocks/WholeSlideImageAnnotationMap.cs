// <copyright file="WholeSlideImageAnnotationMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The whole slide image annotation map.
    /// </summary>
    public class WholeSlideImageAnnotationMap : BaseEntityMap<WholeSlideImageAnnotation>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<WholeSlideImageAnnotation> modelBuilder)
        {
            modelBuilder.ToTable("WholeSlideImageAnnotation", "resources");

            modelBuilder.HasOne(a => a.WholeSlideImage)
                .WithMany(i => i.WholeSlideImageAnnotations)
                .HasForeignKey(a => a.WholeSlideImageId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WholeSlideImageAnnotation_WholeSlideImageId");
        }
    }
}
