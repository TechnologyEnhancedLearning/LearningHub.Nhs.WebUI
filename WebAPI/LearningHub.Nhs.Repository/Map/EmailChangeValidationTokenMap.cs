namespace LearningHub.Nhs.Repository
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Repository.Map;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The email change validation token map.
    /// </summary>
    public class EmailChangeValidationTokenMap : BaseEntityMap<EmailChangeValidationToken>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<EmailChangeValidationToken> modelBuilder)
        {
            modelBuilder.ToTable("EmailChangeValidationToken", "hub");
        }
    }
}