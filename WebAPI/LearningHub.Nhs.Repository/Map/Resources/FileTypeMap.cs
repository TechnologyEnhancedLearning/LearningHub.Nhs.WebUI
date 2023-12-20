// <copyright file="FileTypeMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The file type map.
    /// </summary>
    public class FileTypeMap : BaseEntityMap<FileType>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<FileType> modelBuilder)
        {
            modelBuilder.ToTable("FileType", "resources");

            modelBuilder.Property(e => e.Id).ValueGeneratedNever();

            modelBuilder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(512);

            modelBuilder.Property(e => e.Extension)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(128);

            modelBuilder.HasOne(d => d.DefaultResourceType)
                .WithMany(p => p.FileType)
                .HasForeignKey(d => d.DefaultResourceTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FileType_ResourceType");
        }
    }
}
