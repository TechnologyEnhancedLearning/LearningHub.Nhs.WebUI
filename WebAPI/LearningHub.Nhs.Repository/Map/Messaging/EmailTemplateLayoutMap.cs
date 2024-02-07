// <copyright file="EmailTemplateLayoutMap.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Repository.Map.Messaging
{
    using LearningHub.Nhs.Models.Entities.Messaging;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The EmailTemplateLayoutMap class.
    /// </summary>
    public class EmailTemplateLayoutMap : BaseEntityMap<EmailTemplateLayout>, IEntityTypeMap
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void InternalMap(EntityTypeBuilder<EmailTemplateLayout> modelBuilder)
        {
            modelBuilder.ToTable("EmailTemplateLayout", "messaging");
            modelBuilder.Property(x => x.Name)
                .HasMaxLength(100)
                .IsRequired(false);

            modelBuilder.Property(x => x.Body)
                .IsRequired();
        }
    }
}
