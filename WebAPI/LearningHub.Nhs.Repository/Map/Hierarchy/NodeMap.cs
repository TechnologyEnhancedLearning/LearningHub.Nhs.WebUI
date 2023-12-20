// <copyright file="NodeMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The node map.
    /// </summary>
    public class NodeMap : BaseEntityMap<Node>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Node> modelBuilder)
        {
            modelBuilder.ToTable("Node", "hierarchy");

            modelBuilder.Property(e => e.Description)
                        .IsRequired()
                        .HasMaxLength(512);

            modelBuilder.Property(e => e.Name)
                        .IsRequired()
                        .HasMaxLength(128);

            modelBuilder.HasOne(d => d.CurrentNodeVersion)
                .WithOne(p => p.NodeWhereCurrent)
                .HasForeignKey<Node>(b => b.CurrentNodeVersionId)
                .HasConstraintName("FK_Node_CurrentNodeVersion");

            modelBuilder.Property(e => e.NodeTypeEnum).HasColumnName("NodeTypeId")
               .HasConversion<int>();

            modelBuilder.Property(x => x.Hidden)
                .IsRequired();
        }
    }
}
