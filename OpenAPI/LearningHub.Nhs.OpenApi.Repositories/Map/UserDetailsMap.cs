namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The UserDetailsMap class.
    /// </summary>
    public class UserDetailsMap : BaseEntityMap<UserDetails>, IEntityTypeMap
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<UserDetails> modelBuilder)
        {
            modelBuilder.ToTable("UserDetails", "hub");

            modelBuilder.Property(x => x.FirstName)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            modelBuilder.Property(x => x.EmailAddress)
                .HasMaxLength(100)
                .IsRequired();
        }
    }
}
