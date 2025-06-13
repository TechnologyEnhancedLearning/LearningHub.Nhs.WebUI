namespace LearningHub.Nhs.OpenApi.Repositories.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The FolderNodeVersionMap.
    /// </summary>
    public class FolderNodeVersionMap : BaseEntityMap<FolderNodeVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder.</param>
        protected override void InternalMap(EntityTypeBuilder<FolderNodeVersion> modelBuilder)
        {
            modelBuilder.ToTable("FolderNodeVersion", "hierarchy");

            modelBuilder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(255);

            modelBuilder.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(1800);

            modelBuilder.HasOne(d => d.NodeVersion)
                .WithOne(p => p.FolderNodeVersion)
                .HasForeignKey<FolderNodeVersion>(d => d.NodeVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FolderNodeVersion_NodeVersion");
        }
    }
}
