// <copyright file="AssessmentResourceActivityMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The assessment resource activity event map.
    /// </summary>
    public class AssessmentResourceActivityMap : BaseEntityMap<AssessmentResourceActivity>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<AssessmentResourceActivity> modelBuilder)
        {
            modelBuilder.ToTable("AssessmentResourceActivity", "activity");

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.HasOne(d => d.ResourceActivity)
                .WithMany(p => p.AssessmentResourceActivity)
                .HasForeignKey(d => d.ResourceActivityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_AssessmentResourceActivity_ResourceActivity");
        }
    }
}