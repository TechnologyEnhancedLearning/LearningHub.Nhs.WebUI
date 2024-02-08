namespace LearningHub.Nhs.Repository.Map.Activity
{
    using LearningHub.Nhs.Models.Entities.Activity;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The media resource activity event map.
    /// </summary>
    public class MediaResourcePlayedSegmentMap : BaseEntityMap<MediaResourcePlayedSegment>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<MediaResourcePlayedSegment> modelBuilder)
        {
            modelBuilder.ToTable("MediaResourcePlayedSegment", "activity");
        }
    }
}
