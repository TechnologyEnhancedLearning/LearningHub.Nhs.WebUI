namespace LearningHub.Nhs.Repository.Map.Resources
{
    using LearningHub.Nhs.Models.Entities.Resource;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The question answer map.
    /// </summary>
    public class QuestionAnswerMap : BaseEntityMap<QuestionAnswer>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<QuestionAnswer> modelBuilder)
        {
            modelBuilder.ToTable("QuestionAnswer", "resources");

            modelBuilder.HasKey(e => e.Id)
                .HasName("PK_Resources_QuestionAnswer");

            modelBuilder.HasOne(d => d.BlockCollection)
                .WithOne()
                .HasForeignKey<QuestionAnswer>(d => d.BlockCollectionId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_QuestionAnswer_BlockCollectionId");
        }
    }
}