namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The partial file map.
    /// </summary>
    public class PartialFileMap : BaseEntityMap<PartialFile>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<PartialFile> modelBuilder)
        {
            modelBuilder.ToTable("PartialFile", "resources");

            modelBuilder.Property(e => e.Id);

            modelBuilder.Property(e => e.TotalSize)
                .IsRequired();

            modelBuilder.HasOne(d => d.File)
                .WithOne(p => p.PartialFile)
                .HasForeignKey<PartialFile>(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PartialFile_FileId");
        }
    }
}
