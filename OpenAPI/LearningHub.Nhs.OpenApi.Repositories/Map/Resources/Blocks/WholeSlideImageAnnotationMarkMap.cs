// <copyright file="WholeSlideImageAnnotationMarkMap.cs" company="NHS England">
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
    public class WholeSlideImageAnnotationMarkMap : BaseEntityMap<WholeSlideImageAnnotationMark>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<WholeSlideImageAnnotationMark> modelBuilder)
        {
            modelBuilder.ToTable("WholeSlideImageAnnotationMark", "resources");

            modelBuilder.HasOne(a => a.WholeSlideImageAnnotation)
                .WithMany(i => i.WholeSlideImageAnnotationMarks)
                .HasForeignKey(a => a.WholeSlideImageAnnotationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_WholeSlideImageAnnotationMark_WholeSlideImageAnnotationId");
        }
    }
}
