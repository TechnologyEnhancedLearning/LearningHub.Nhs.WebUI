namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The user map.
    /// </summary>
    public class UserMap : BaseEntityMap<User>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<User> modelBuilder)
        {
            modelBuilder.ToTable("User", "hub");

            modelBuilder.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            modelBuilder.Ignore(e => e.AssignedRoles);
        }
    }
}
