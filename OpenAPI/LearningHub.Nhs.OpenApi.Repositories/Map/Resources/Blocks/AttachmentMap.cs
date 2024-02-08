namespace LearningHub.Nhs.OpenApi.Repositories.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource.Blocks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The attachment map.
    /// </summary>
    public class AttachmentMap : BaseEntityMap<Attachment>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<Attachment> modelBuilder)
        {
            modelBuilder.ToTable("Attachment", "resources");

            modelBuilder.HasOne(d => d.File)
                .WithMany(p => p.Attachments)
                .HasForeignKey(d => d.FileId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Attachment_FileId");
        }
    }
}
