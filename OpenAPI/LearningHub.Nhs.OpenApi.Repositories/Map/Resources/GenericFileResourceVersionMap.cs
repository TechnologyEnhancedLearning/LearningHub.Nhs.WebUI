// <copyright file="GenericFileResourceVersionMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The generic file resource version map.
    /// </summary>
    public class GenericFileResourceVersionMap : BaseEntityMap<GenericFileResourceVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<GenericFileResourceVersion> modelBuilder)
        {
            modelBuilder.HasKey(e => e.Id)
                    .HasName("PK_Resources_GenericFileResourceVersion");

            modelBuilder.ToTable("GenericFileResourceVersion", "resources");

            modelBuilder.Property(e => e.ResourceVersionId).ValueGeneratedNever();
            modelBuilder.HasAlternateKey(c => c.ResourceVersionId);

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.GenericFileResourceVersion)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GenericFileResource_File");

            modelBuilder.HasOne(d => d.ResourceVersion)
                .WithOne(p => p.GenericFileResourceVersion)
                .HasForeignKey<GenericFileResourceVersion>(d => d.ResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_GenericFileResourceVersion_ResourceVersion");
        }
    }
}
