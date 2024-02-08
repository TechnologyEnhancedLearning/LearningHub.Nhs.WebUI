namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The node resource map.
    /// </summary>
    public class NodeResourceMap : BaseEntityMap<NodeResource>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<NodeResource> modelBuilder)
        {
            modelBuilder.ToTable("NodeResource", "hierarchy");

            modelBuilder.HasOne(d => d.Node)
                .WithMany(p => p.NodeResource)
                .HasForeignKey(d => d.NodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodeResource_Node");

            modelBuilder.HasOne(d => d.Resource)
                .WithMany(p => p.NodeResource)
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodeResource_Resource");

            modelBuilder.Property(e => e.VersionStatusEnum).HasColumnName("VersionStatusId")
               .HasConversion<int>();
        }
    }
}
