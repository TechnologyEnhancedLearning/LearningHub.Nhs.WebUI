// <copyright file="WholeSlideImageMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The whole slide image map.
    /// </summary>
    public class WholeSlideImageMap : BaseEntityMap<WholeSlideImage>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<WholeSlideImage> modelBuilder)
        {
            modelBuilder.ToTable("WholeSlideImage", "resources");

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.WholeSlideImages)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WholeSlideImage_FileId");
        }
    }
}
