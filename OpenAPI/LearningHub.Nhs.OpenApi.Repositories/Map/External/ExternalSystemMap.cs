// <copyright file="ExternalSystemMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.External
{
    using LearningHub.Nhs.Models.Entities.External;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The ExternalSystem map.
    /// </summary>
    public class ExternalSystemMap : BaseEntityMap<ExternalSystem>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ExternalSystem> modelBuilder)
        {
            modelBuilder.ToTable("ExternalSystem", "external");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.Name).HasMaxLength(100);

            modelBuilder.Property(e => e.Code).HasMaxLength(100);

            modelBuilder.Property(e => e.CallbackUrl).HasMaxLength(512);

            modelBuilder.Property(e => e.SecretKey).HasMaxLength(100);
        }
    }
}
