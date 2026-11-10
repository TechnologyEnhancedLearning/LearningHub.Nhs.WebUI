using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using LearningHub.Nhs.Models.Entities;

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    /// <summary>
    /// The base entity map.
    /// </summary>
    /// <typeparam name="TEntityType">Input type.</typeparam>
    public abstract class AuditableEntityMap<TEntityType> : IEntityTypeMap
     where TEntityType : AuditableEntityBase
    {
        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The builder.
        /// </param>
        public void Map(ModelBuilder modelBuilder)
        {
            var builder = modelBuilder.Entity<TEntityType>();

            // New audit structure does not persist the legacy Deleted property.
            builder.Ignore(e => e.Deleted);

            // Exclude removed records by default.
            builder.HasQueryFilter(e => e.RemoveDate == null);

            this.InternalMap(builder);
        }

        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        protected abstract void InternalMap(
            EntityTypeBuilder<TEntityType> builder);
    }
}
