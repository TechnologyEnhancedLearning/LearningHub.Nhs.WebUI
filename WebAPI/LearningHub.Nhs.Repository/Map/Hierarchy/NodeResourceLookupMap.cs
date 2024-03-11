namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The node resource lookup map.
    /// </summary>
    public class NodeResourceLookupMap : BaseEntityMap<NodeResourceLookup>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<NodeResourceLookup> modelBuilder)
        {
            modelBuilder.ToTable("NodeResourceLookup", "hierarchy");

            modelBuilder.HasOne(d => d.Node)
                .WithMany()
                .HasForeignKey(d => d.NodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodeResourceLookup_Node");

            modelBuilder.HasOne(d => d.Resource)
                .WithMany()
                .HasForeignKey(d => d.ResourceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodeResourceLookup_Resource");
        }
    }
}
