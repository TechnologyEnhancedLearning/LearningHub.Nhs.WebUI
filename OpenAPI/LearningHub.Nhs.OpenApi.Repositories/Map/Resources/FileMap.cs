namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using System;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The file map.
    /// </summary>
    public class FileMap : BaseEntityMap<File>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<File> modelBuilder)
        {
            modelBuilder.ToTable("File", "resources");

            modelBuilder.Property(e => e.Id);

            modelBuilder.Property(e => e.FileName)
                .IsRequired()
                .HasMaxLength(128);

            modelBuilder.Property(e => e.FilePath)
                .IsRequired()
                .HasMaxLength(1024);

            modelBuilder.HasOne(d => d.FileType)
                .WithMany(p => p.File)
                .HasForeignKey(d => d.FileTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_File_FileType");
        }
    }
}
