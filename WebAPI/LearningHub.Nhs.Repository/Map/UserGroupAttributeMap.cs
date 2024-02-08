namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The UserGroupAttribute map.
    /// </summary>
    public class UserGroupAttributeMap : BaseEntityMap<UserGroupAttribute>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<UserGroupAttribute> modelBuilder)
        {
            modelBuilder.ToTable("UserGroupAttribute", "hub");

            modelBuilder.Property(e => e.TextValue).HasMaxLength(255);

            modelBuilder.HasOne(d => d.Attribute)
                    .WithMany()
                    .HasForeignKey(d => d.AttributeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserGroupAttribute_Attribute");

            modelBuilder.HasOne(d => d.UserGroup)
                    .WithMany(p => p.UserGroupAttribute)
                    .HasForeignKey(d => d.UserGroupId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserGroupAttribute_UserGroup");

            modelBuilder.HasOne(d => d.Scope)
                .WithMany()
                .HasForeignKey(d => d.ScopeId)
                .HasConstraintName("FK_UserGroupAttribute_Scope");
        }
    }
}
