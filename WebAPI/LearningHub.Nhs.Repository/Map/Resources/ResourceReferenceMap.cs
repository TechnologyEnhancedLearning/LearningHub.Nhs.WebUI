namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource reference map.
    /// </summary>
    public class ResourceReferenceMap : BaseEntityMap<ResourceReference>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceReference> modelBuilder)
        {
            modelBuilder.ToTable("ResourceReference", "resources");

            modelBuilder.HasOne(d => d.Resource)
                        .WithMany(p => p.ResourceReference)
                        .HasForeignKey(d => d.ResourceId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ResourceReference_Resource");

            modelBuilder.HasOne(d => d.NodePath)
                        .WithMany(p => p.ResourceReference)
                        .HasForeignKey(d => d.NodePathId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ResourceReference_NodePath");
        }
    }
}
