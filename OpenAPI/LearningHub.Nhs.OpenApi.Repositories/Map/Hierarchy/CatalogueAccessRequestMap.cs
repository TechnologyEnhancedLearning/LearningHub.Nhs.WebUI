namespace LearningHub.Nhs.OpenApi.Repositories.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The CatalogueAccessRequestMap class.
    /// </summary>
    public class CatalogueAccessRequestMap : BaseEntityMap<CatalogueAccessRequest>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The modelBuilder.</param>
        protected override void InternalMap(EntityTypeBuilder<CatalogueAccessRequest> modelBuilder)
        {
            modelBuilder.ToTable("CatalogueAccessRequest", "hierarchy");

            modelBuilder.Property(x => x.UserId)
                .IsRequired();

            modelBuilder.Property(x => x.CatalogueNodeId)
                .IsRequired();

            modelBuilder.Property(x => x.EmailAddress)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Property(x => x.Message)
                .IsRequired();

            modelBuilder.Property(x => x.ResponseMessage)
                .IsRequired(false);

            modelBuilder.Property(x => x.CompletedDate)
                .IsRequired(false);

            modelBuilder.HasOne(x => x.User)
                .WithOne()
                .HasForeignKey<CatalogueAccessRequest>(x => x.UserId)
                .HasConstraintName("FK_CatalogueAccessRequest_User");

            modelBuilder.HasOne(x => x.UserProfile)
                .WithOne()
                .HasForeignKey<CatalogueAccessRequest>(x => x.UserId);
                ////.HasConstraintName("FK_CatalogueAccessRequest_UserDetails");
        }
    }
}
