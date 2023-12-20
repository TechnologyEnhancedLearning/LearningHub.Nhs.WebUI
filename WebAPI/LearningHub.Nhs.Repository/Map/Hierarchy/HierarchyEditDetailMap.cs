// <copyright file="HierarchyEditDetailMap.cs" company="HEE.nhs.uk">
// Copyright (c) HEE.nhs.uk.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Hierarchy
{
    using LearningHub.Nhs.Models.Entities.Hierarchy;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The hierarchy edit map.
    /// </summary>
    public class HierarchyEditDetailMap : BaseEntityMap<HierarchyEditDetail>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<HierarchyEditDetail> modelBuilder)
        {
            modelBuilder.ToTable("HierarchyEditDetail", "hierarchy");

            modelBuilder.HasOne(d => d.HierarchyEdit)
                .WithMany(p => p.HierarchyEditDetail)
                .HasForeignKey(d => d.HierarchyEditId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_HierarchyEditDetail_HierarchyEdit");

            modelBuilder.Property(e => e.HierarchyEditDetailType).HasColumnName("HierarchyEditDetailTypeId")
               .HasConversion<int>();

            modelBuilder.Property(e => e.HierarchyEditDetailOperation).HasColumnName("HierarchyEditDetailOperationId")
               .HasConversion<int>();
        }
    }
}
