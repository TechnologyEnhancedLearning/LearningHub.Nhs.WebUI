// <copyright file="NodePathNodeMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The node path node map.
    /// </summary>
    public class NodePathNodeMap : BaseEntityMap<NodePathNode>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<NodePathNode> modelBuilder)
        {
            modelBuilder.ToTable("NodePathNode", "hierarchy");

            modelBuilder.HasOne(d => d.Node)
                .WithMany(p => p.NodePathNodes)
                .HasForeignKey(d => d.NodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodePathNode_Node");

            modelBuilder.HasOne(d => d.NodePath)
                .WithMany(p => p.NodePathNode)
                .HasForeignKey(d => d.NodePathId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodePathNode_NodePath");
        }
    }
}
