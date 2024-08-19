namespace LearningHub.Nhs.OpenApi.Repositories.Map.Hierarchy
{
    using System;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The node path display map.
    /// </summary>
    public class NodePathDisplayMap : BaseEntityMap<NodePathDisplay>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<NodePathDisplay> modelBuilder)
        {
            modelBuilder.ToTable("NodePathDisplay", "hierarchy");

            modelBuilder.Property(e => e.NodePathId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.NodePathId);

            modelBuilder.Property(e => e.DisplayName).HasMaxLength(128);

            modelBuilder.Property(e => e.Icon).HasMaxLength(128);

            modelBuilder.HasOne(d => d.NodePath)
                        .WithOne(p => p.NodePathDisplay)
                        .HasForeignKey<NodePath>(b => b.Id)
                        .HasConstraintName("FK_NodePathDisplay_NodePath");
        }
    }
}
