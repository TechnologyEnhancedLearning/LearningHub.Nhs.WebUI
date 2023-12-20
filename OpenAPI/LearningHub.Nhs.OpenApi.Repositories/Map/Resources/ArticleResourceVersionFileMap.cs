// <copyright file="ArticleResourceVersionFileMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using System;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The article resource version file map.
    /// </summary>
    public class ArticleResourceVersionFileMap : BaseEntityMap<ArticleResourceVersionFile>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ArticleResourceVersionFile> modelBuilder)
        {
            modelBuilder.ToTable("ArticleResourceVersionFile", "resources");

            modelBuilder.HasOne(d => d.ArticleResourceVersion)
                .WithMany(p => p.ArticleResourceVersionFile)
                .HasForeignKey(d => d.ArticleResourceVersionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleResourceVersionFile_ArticleResourceVersion");

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.ArticleResourceVersionFile)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ArticleResourceVersionFile_File");
        }
    }
}
