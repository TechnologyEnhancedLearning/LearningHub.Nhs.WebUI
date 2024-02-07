// <copyright file="WholeSlideImageFileMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The whole slide image file map.
    /// </summary>
    public class WholeSlideImageFileMap : BaseEntityMap<WholeSlideImageFile>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<WholeSlideImageFile> modelBuilder)
        {
            modelBuilder.ToTable("WholeSlideImageFile", "resources");

            modelBuilder.Property(e => e.Id);

            modelBuilder.Property(e => e.Status)
                .IsRequired();

            modelBuilder.HasOne(d => d.File)
                .WithOne(p => p.WholeSlideImageFile)
                .HasForeignKey<WholeSlideImageFile>(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_WholeSlideImageFile_FileId");
        }
    }
}
