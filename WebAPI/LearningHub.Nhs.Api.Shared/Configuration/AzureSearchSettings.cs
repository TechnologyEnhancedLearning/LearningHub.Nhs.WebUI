using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Api.Shared.Configuration
{
    /// <summary>
    /// The Azure AI Search configuration settings.
    /// </summary>
    public class AzureSearchSettings
    {
        /// <summary>
        /// Gets or sets the maximum description length.
        /// </summary>
        public int MaximumDescriptionLength { get; set; } = 150;
    }
}
