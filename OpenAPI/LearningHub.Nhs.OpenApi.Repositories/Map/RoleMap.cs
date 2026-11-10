namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The role map.
    /// </summary>
    public class RoleMap : AuditableEntityMap<Role>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Role> modelBuilder)
        {
            modelBuilder.ToTable("Role", "hub");

            modelBuilder.Property(e => e.Code)
                .HasColumnName("Code")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.Name)
                .IsRequired()
                .HasColumnName("Name")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.ScopeType)
                .HasColumnName("ScopeType")
                .HasMaxLength(250);

            modelBuilder.Property(e => e.Description)
                .HasColumnName("Description")
                .HasMaxLength(500);
        }
    }
}
