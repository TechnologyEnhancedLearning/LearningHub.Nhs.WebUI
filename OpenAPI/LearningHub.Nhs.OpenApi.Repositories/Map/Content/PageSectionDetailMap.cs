namespace LearningHub.Nhs.OpenApi.Repositories.Map.Content
{
    using LearningHub.Nhs.Models.Entities.Content;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the <see cref="PageSectionDetailMap" />.
    /// </summary>
    public class PageSectionDetailMap : BaseEntityMap<PageSectionDetail>
    {
        /// <summary>
        /// The InternalMap.
        /// </summary>
        /// <param name="entity">The entity<see cref="EntityTypeBuilder{PageSectionDetail}"/>.</param>
        protected override void InternalMap(EntityTypeBuilder<PageSectionDetail> entity)
        {
            entity.ToTable("PageSectionDetail", "content");

            entity.Property(e => e.BackgroundColour).HasMaxLength(20);

            entity.Property(e => e.Description).HasMaxLength(512);

            entity.Property(e => e.HyperLinkColour).HasMaxLength(20);

            entity.Property(e => e.PageSectionStatusId).HasDefaultValueSql("((1))");

            entity.Property(e => e.SectionLayoutTypeId).HasDefaultValueSql("((1))");

            entity.Property(e => e.TextColour).HasMaxLength(20);

            entity.Property(e => e.SectionTitle).HasMaxLength(128);

            entity.Property(e => e.DeletePending).IsRequired(false);

            entity.Property(e => e.DraftHidden).IsRequired(false);

            entity.Property(e => e.DraftPosition).IsRequired(false);

            entity.HasOne(d => d.PageSection)
                .WithMany(p => p.PageSectionDetails)
                .HasForeignKey(d => d.PageSectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PageSectionDetail_PageSection");
        }
    }
}
