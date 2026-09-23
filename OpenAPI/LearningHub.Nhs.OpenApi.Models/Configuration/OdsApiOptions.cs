using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// Configuration for the NHS Organisation Data Service API.
    /// </summary>
    public class OdsApiOptions
    {
        /// <summary>
        /// Gets or sets the API base URL.
        /// </summary>
        public string BaseUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the API key.
        /// </summary>
        public string? ApiKey { get; set; }
    }
}
