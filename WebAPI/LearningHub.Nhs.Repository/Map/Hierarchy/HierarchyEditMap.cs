namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The hierarchy edit map.
    /// </summary>
    public class HierarchyEditMap : BaseEntityMap<HierarchyEdit>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<HierarchyEdit> modelBuilder)
        {
            modelBuilder.ToTable("HierarchyEdit", "hierarchy");

            modelBuilder.Property(e => e.HierarchyEditStatus).HasColumnName("HierarchyEditStatusId")
               .HasConversion<int>();

            modelBuilder.HasOne(d => d.CreateUser)
                .WithMany(p => p.HierarchyEdit)
                .HasForeignKey(d => d.CreateUserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HierarchyEdit_CreateUser");

            modelBuilder.HasOne(d => d.RootNodePath)
                .WithMany()
                .HasForeignKey(d => d.RootNodePathId)
                .HasConstraintName("FK_HierarchyEdit_NodePath");

            modelBuilder.HasOne(d => d.Publication)
                .WithMany()
                .HasForeignKey(d => d.PublicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HierarchyEdit_Publication");
        }
    }
}
