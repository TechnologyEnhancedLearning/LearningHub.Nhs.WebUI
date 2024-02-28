namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using System;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The resource version rating summary map.
    /// </summary>
    public class ResourceVersionRatingSummaryMap : BaseEntityMap<ResourceVersionRatingSummary>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ResourceVersionRatingSummary> modelBuilder)
        {
            modelBuilder.ToTable("ResourceVersionRatingSummary", "resources");

            modelBuilder.HasOne(d => d.ResourceVersion)
                        .WithOne(p => p.ResourceVersionRatingSummary)
                        .HasForeignKey<ResourceVersionRatingSummary>(d => d.ResourceVersionId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_ResourceVersionRatingSummary_ResourceVersion");
        }
    }
}
