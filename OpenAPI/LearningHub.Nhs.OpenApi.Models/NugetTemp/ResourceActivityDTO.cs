using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Models.NugetTemp
{
    // ResourceActivity Entity without virtuals
    public class ResourceActivityDTO //: EntityBase qqqq
    {
        //
        // Summary:
        //     Gets or sets the Id.
        public int Id { get; set; }


        //
        // Summary:
        //     Gets or sets the UserId.
        public int UserId { get; set; }

        //
        // Summary:
        //     Gets or sets the launch resource activity id.
        public int? LaunchResourceActivityId { get; set; }

        //
        // Summary:
        //     Gets or sets the ResourceId.
        public int ResourceId { get; set; }

        //
        // Summary:
        //     Gets or sets the ResourceVersionId.
        public int ResourceVersionId { get; set; }

     

        //
        // Summary:
        //     Gets or sets the MajorVersion.
        public int MajorVersion { get; set; }

        //
        // Summary:
        //     Gets or sets the MinorVersion.
        public int MinorVersion { get; set; }

        //
        // Summary:
        //     Gets or sets the NodePathId.
        public int NodePathId { get; set; }

        //
        // Summary:
        //     Gets or sets the ActivityStatusId.
        public int ActivityStatusId { get; set; }

        //
        // Summary:
        //     Gets or sets the ActivityStart.
        public DateTimeOffset? ActivityStart { get; set; }

        //
        // Summary:
        //     Gets or sets the ActivityEnd.
        public DateTimeOffset? ActivityEnd { get; set; }

        //
        // Summary:
        //     Gets or sets the DurationSeconds.
        public int DurationSeconds { get; set; }

        //
        // Summary:
        //     Gets or sets the Score.
        public decimal? Score { get; set; }
    }
}
