using LearningHub.Nhs.Models.Entities.DatabricksReport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearningHub.Nhs.OpenApi.Repositories.Map
{
    /// <summary>
    /// The ReportHistory Map.
    /// </summary>
    public class ReportHistoryMap : BaseEntityMap<ReportHistory>
    {
        /// <summary>
        /// The internal map.
        /// </summary>
        /// <param name="modelBuilder">
        /// The model builder.
        /// </param>
        protected override void InternalMap(EntityTypeBuilder<ReportHistory> modelBuilder)
        {
            modelBuilder.ToTable("ReportHistory", "Reports");
        }
    }
}
