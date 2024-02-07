// <copyright file="ExternalSystemDeepLinkMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.External
{
    using LearningHub.Nhs.Models.Entities.External;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The ExternalSystemDeepLink map.
    /// </summary>
    public class ExternalSystemDeepLinkMap : BaseEntityMap<ExternalSystemDeepLink>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ExternalSystemDeepLink> modelBuilder)
        {
            modelBuilder.ToTable("ExternalSystemDeepLink", "external");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.Code).HasMaxLength(100);

            modelBuilder.Property(e => e.DeepLink).HasMaxLength(512);
        }
    }
}
