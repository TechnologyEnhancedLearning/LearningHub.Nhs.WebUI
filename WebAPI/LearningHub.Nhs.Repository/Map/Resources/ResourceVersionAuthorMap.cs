namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource version author map.
    /// </summary>
    public class ResourceVersionAuthorMap : BaseEntityMap<ResourceVersionAuthor>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionAuthor> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersionAuthor", "resources");

            modelBuilder.Property(e => e.AuthorName).HasMaxLength(100);
            modelBuilder.Property(e => e.Organisation).HasMaxLength(100);
            modelBuilder.Property(e => e.Role).HasMaxLength(100);

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithMany(p => p.ResourceVersionAuthor)
                .HasForeignKey(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ResourceVersionAuthor_ResourceVersion");
        }
    }
}
