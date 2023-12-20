// <copyright file="NodeVersionMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.OpenApi.Repositories.Map.Hierarchy
{
    using System;
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The node version map.
    /// </summary>
    public class NodeVersionMap : BaseEntityMap<NodeVersion>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<NodeVersion> modelBuilder)
        {
            modelBuilder.ToTable("NodeVersion", "hierarchy");

            modelBuilder.Property(x => x.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.HasOne(d => d.Node)
                    .WithMany(p => p.NodeVersion)
                    .HasForeignKey(d => d.NodeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NodeVersion_Node");

            modelBuilder.Property(e => e.VersionStatusEnum).HasColumnName("VersionStatusId")
               .HasConversion<int>();
        }
    }
}
