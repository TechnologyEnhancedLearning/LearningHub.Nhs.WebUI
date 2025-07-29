namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using LearningHub.Nhs.Models.Entities.Resource;
    using LearningHub.Nhs.OpenApi.Repositories.Interface.Map;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The base entity map.
    /// </summary>
    /// <typeparam name="TEntityType">Input type.</typeparam>
    public abstract class BaseEntityMap<TEntityType> : IEntityTypeMap
       where TEntityType : EntityBase
    {
        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Map(ModelBuilder builder)
        {
            builder.Entity<TEntityType>().Property(e => e.Id).HasColumnName("Id");

            builder.Entity<TEntityType>().Property(e => e.AmendDate).HasColumnName("CreateDate");

            builder.Entity<TEntityType>().Property(e => e.AmendUserId).HasColumnName("CreateUserId");

            builder.Entity<TEntityType>().Property(e => e.AmendDate).HasColumnName("AmendDate");

            builder.Entity<TEntityType>().Property(e => e.AmendUserId).HasColumnName("AmendUserId");

            builder.Entity<TEntityType>().Property(e => e.Deleted).HasColumnName("Deleted");

            if (typeof(TEntityType) != typeof(ArticleResourceVersion)
                && typeof(TEntityType) != typeof(AudioResourceVersion)
                && typeof(TEntityType) != typeof(EmbeddedResourceVersion)
                && typeof(TEntityType) != typeof(EquipmentResourceVersion)
                && typeof(TEntityType) != typeof(GenericFileResourceVersion)
                && typeof(TEntityType) != typeof(HtmlResourceVersion)
                && typeof(TEntityType) != typeof(ImageResourceVersion)
                && typeof(TEntityType) != typeof(VideoResourceVersion)
                && typeof(TEntityType) != typeof(WebLinkResourceVersion)
                && typeof(TEntityType) != typeof(ResourceLicence)
                && typeof(TEntityType) != typeof(User)
                && typeof(TEntityType) != typeof(UserBookmark))
            {
                builder.Entity<TEntityType>().HasQueryFilter(e => !e.Deleted);
            }

            //// NodePathId is now populated for all ResourceVersions
            ////if (typeof(TEntityType) == typeof(ResourceReference))
            ////{
            ////    builder.Entity<ResourceReference>().HasQueryFilter(e => !e.NodePathId.HasValue);
            ////}

            this.InternalMap(builder.Entity<TEntityType>());
        }

        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        protected abstract void InternalMap(EntityTypeBuilder<TEntityType> builder);
    }
}
