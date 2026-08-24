namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The user map.
    /// </summary>
    public class UserMap : AuditableEntityMap<User>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<User> modelBuilder)
        {
            modelBuilder.ToTable("User", "hub");

            modelBuilder.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedNever();

            modelBuilder.Property(e => e.FirstName)
           .HasColumnName("FirstName")
           .HasMaxLength(50);

            modelBuilder.Property(e => e.LastName)
                .HasColumnName("LastName")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.EmailAddress)
                .HasColumnName("EmailAddress")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.RecoveryEmailAddress)
                .HasColumnName("RecoveryEmailAddress")
                .HasMaxLength(100);

            modelBuilder.Property(e => e.LegacyUserName)
                .IsRequired()
                .HasColumnName("LegacyUserName")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.ProfessionalBodyId)
                .HasColumnName("ProfessionalBodyId");

            modelBuilder.Property(e => e.ProfessionalRegistrationNumber)
                .HasColumnName("ProfessionalRegistrationNumber")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.Active)
                .HasColumnName("Active");

            modelBuilder.Property(e => e.PasswordHash)
                .HasColumnName("PasswordHash")
                .HasMaxLength(255);

            modelBuilder.Property(e => e.MustChangePassword)
                .HasColumnName("MustChangePassword");

            modelBuilder.Property(e => e.PasswordLifeCounter)
           .HasColumnName("PasswordLifeCounter");

            modelBuilder.Property(e => e.SecurityLifeCounter)
                .HasColumnName("SecurityLifeCounter");

            modelBuilder.Property(e => e.RemoteLoginKey)
                .HasColumnName("RemoteLoginKey")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.RemoteLoginGuid)
                .HasColumnName("RemoteLoginGuid");

            modelBuilder.Property(e => e.RemoteLoginStart)
                .HasColumnName("RemoteLoginStart");

            modelBuilder.Property(e => e.RestrictToSSO)
                .HasColumnName("RestrictToSSO");

            modelBuilder.Property(e => e.RequestUserLogout)
                .HasColumnName("RequestUserLogout");

            modelBuilder.Property(e => e.RemovalMethodId)
                .HasColumnName("RemovalMethodId");
        }
    }
}
