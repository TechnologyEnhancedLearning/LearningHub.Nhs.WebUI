// <copyright file="NodePathMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The node path map.
    /// </summary>
    public class NodePathMap : BaseEntityMap<NodePath>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<NodePath> modelBuilder)
        {
            modelBuilder.ToTable("NodePath", "hierarchy");

            modelBuilder.Property(e => e.NodePathString)
                .IsRequired()
                .HasColumnName("NodePath")
                .HasMaxLength(256);

            modelBuilder.HasOne(d => d.Node)
                .WithMany(p => p.NodePaths)
                .HasForeignKey(d => d.NodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodePath_Node");

            modelBuilder.HasOne(d => d.Node)
                .WithMany(p => p.NodePaths)
                .HasForeignKey(d => d.NodeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NodePath_CatalogueNode");
        }
    }
}
