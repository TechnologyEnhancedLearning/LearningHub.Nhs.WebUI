namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    using LearningHub.Nhs.Models.Entities;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// The log map.
    /// </summary>
    public class LogMap : IEntityTypeMap // BaseEntityMap<Log>
    {
        /// <summary>
        /// The map.
        /// </summary>
        /// <param name="builder">
        /// The builder.
        /// </param>
        public void Map(ModelBuilder builder)
        {
            this.InternalMap(builder.Entity<Log>());
        }

        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected void InternalMap(EntityTypeBuilder<Log> modelBuilder)
        {
            modelBuilder.ToTable("Log", "hub");

            modelBuilder.HasKey(e => e.Id);

            modelBuilder.Property(e => e.Id)
                    .HasColumnName("Id")
                    .ValueGeneratedNever();

            modelBuilder.Property(e => e.Application)
                .IsRequired()
                .HasColumnName("Application")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.Logged)
                .IsRequired()
                .HasColumnName("Logged")
                .HasMaxLength(1024);

            modelBuilder.Property(e => e.Level)
                .IsRequired()
                .HasColumnName("Level")
                .HasMaxLength(50);

            modelBuilder.Property(e => e.Message)
                .IsRequired()
                .HasColumnName("Message");

            modelBuilder.Property(e => e.Logger)
                .HasColumnName("Logger")
                .HasMaxLength(250);

            modelBuilder.Property(e => e.Callsite)
                .HasColumnName("Callsite");

            modelBuilder.Property(e => e.Exception)
                .HasColumnName("Exception");

            modelBuilder.Property(e => e.UserId)
                .HasColumnName("UserId");
        }
    }
}
