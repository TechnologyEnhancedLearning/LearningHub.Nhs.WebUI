// <copyright file="ScopeMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The Scope map.
    /// </summary>
    public class ScopeMap : BaseEntityMap<Scope>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<Scope> modelBuilder)
        {
            modelBuilder.ToTable("Scope", "hub");

            modelBuilder.HasOne(d => d.CatalogueNode)
                .WithMany()
                .HasForeignKey(d => d.CatalogueNodeId)
                .HasConstraintName("FK_Scope_CatalogueNode");

            modelBuilder.Property(e => e.ScopeType).HasColumnName("ScopeTypeId")
               .HasConversion<int>();
        }
    }
}
