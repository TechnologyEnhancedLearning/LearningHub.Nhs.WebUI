namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Entities.Resource;
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Scorm resource version map.
    /// </summary>
    public class ExternalReferenceUserAgreementMap : BaseEntityMap<ExternalReferenceUserAgreement>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ExternalReferenceUserAgreement> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_ExternalReferenceUserAgreement");

            modelBuilder.ToTable("ExternalReferenceUserAgreement", "resources");

            modelBuilder.Property(e => e.ExternalReferenceId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ExternalReferenceId);
        }
    }
}
