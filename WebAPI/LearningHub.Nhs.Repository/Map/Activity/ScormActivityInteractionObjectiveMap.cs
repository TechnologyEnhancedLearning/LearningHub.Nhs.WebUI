namespace LearningHub.Nhs.Repository.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The scorm activity interaction objective map.
    /// </summary>
    public class ScormActivityInteractionObjectiveMap : BaseEntityMap<ScormActivityInteractionObjective>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<ScormActivityInteractionObjective> modelBuilder)
        {
            modelBuilder.ToTable("ScormActivityInteractionObjective", "activity");

            modelBuilder.Property(e => e.ObjectiveId).HasMaxLength(255);

            modelBuilder.Property(e => e.AmendUserId).HasColumnName("AmendUserID");

            modelBuilder.Property(e => e.CreateUserId).HasColumnName("CreateUserID");

            modelBuilder.HasOne(d => d.ScormActivityInteraction)
                .WithMany(p => p.ScormActivityInteractionObjective)
                .HasForeignKey(d => d.ScormActivityInteractionId)
                .HasConstraintName("FK_ScormActivityInteractionObjective_ScormActivityInteraction");
        }
    }
}
