namespace LearningHub.Nhs.OpenApi.Repositories.Map.Migrations
{
    using System;
    using LearningHub.Nhs.Models.Entities.Migration;
    using LearningHub.Nhs.Models.Enums;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The migration input record map.
    /// </summary>
    public class MigrationInputRecordMap : BaseEntityMap<MigrationInputRecord>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<MigrationInputRecord> modelBuilder)
        {
            modelBuilder.ToTable("MigrationInputRecord", "migrations");

            modelBuilder.Property(e => e.MigrationInputRecordStatusEnum).HasColumnName("MigrationInputRecordStatusId")
               .HasConversion<int>();

            modelBuilder.HasOne(d => d.Migration)
                .WithMany(p => p.MigrationInputRecords)
                .HasForeignKey(d => d.MigrationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_MigrationInputRecord_Migration");
        }
    }
}
