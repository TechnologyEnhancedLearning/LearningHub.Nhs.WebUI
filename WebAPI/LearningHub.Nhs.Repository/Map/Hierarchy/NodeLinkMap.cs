namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The node link map.
    /// </summary>
    public class NodeLinkMap : BaseEntityMap<NodeLink>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<NodeLink> modelBuilder)
        {
            modelBuilder.ToTable("NodeLink", "hierarchy");

            modelBuilder.HasOne(d => d.ChildNode)
                .WithMany(p => p.NodeLinkChildNode)
                .HasForeignKey(d => d.ChildNodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodeLink_ChildNode");

            modelBuilder.HasOne(d => d.ParentNode)
                .WithMany(p => p.NodeLinkParentNode)
                .HasForeignKey(d => d.ParentNodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodeLink_ParentNode");
        }
    }
}
