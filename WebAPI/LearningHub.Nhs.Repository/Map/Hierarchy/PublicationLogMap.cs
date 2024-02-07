// <copyright file="PublicationLogMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The publication log map.
    /// </summary>
    public class PublicationLogMap : BaseEntityMap<PublicationLog>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<PublicationLog> modelBuilder)
        {
            modelBuilder.ToTable("PublicationLog", "hierarchy");

            modelBuilder.HasOne(d => d.Publication)
                .WithMany(p => p.PublicationLog)
                .HasForeignKey(d => d.PublicationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PublicationLog_Publication");
        }
    }
}