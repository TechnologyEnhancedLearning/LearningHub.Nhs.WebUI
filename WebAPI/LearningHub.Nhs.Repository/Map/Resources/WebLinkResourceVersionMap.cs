// <copyright file="WebLinkResourceVersionMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The web link resource version map.
    /// </summary>
    public class WebLinkResourceVersionMap : BaseEntityMap<WebLinkResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<WebLinkResourceVersion> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_WebLinkResourceVersion");

            modelBuilder.ToTable("WebLinkResourceVersion", "resources");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.Property(e => e.DisplayText)
                    .IsRequired()
                    .HasMaxLength(50);

            modelBuilder.Property(e => e.WebLinkUrl)
                    .IsRequired()
                    .HasColumnName("WebLinkURL")
                    .HasMaxLength(1024);

            modelBuilder.HasOne(d => d.ResourceVersion)
                    .WithOne(p => p.WebLinkResourceVersion)
                    .HasForeignKey<WebLinkResourceVersion>(d => d.ResourceVersionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WebLinkResourceVersion_ResourceVersion");
        }
    }
}