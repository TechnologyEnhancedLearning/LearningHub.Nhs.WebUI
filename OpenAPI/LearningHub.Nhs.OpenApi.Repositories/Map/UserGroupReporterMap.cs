using LearningHub.Nhs.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    /// <summary>
    /// The User group reporter map
    /// </summary>
    public class UserGroupReporterMap : BaseEntityMap<UserGroupReporter>
    {

        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<UserGroupReporter> builder)
        {
            builder.ToTable("UserGroupReporter", "hub");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.UserGroupId)
                .IsRequired();

            builder.Property(x => x.Deleted)
                .IsRequired();

            builder.Property(x => x.AmendUserId);

            builder.Property(x => x.AmendDate);

            builder.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UserGroup)
                .WithMany()
                .HasForeignKey(x => x.UserGroupId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
