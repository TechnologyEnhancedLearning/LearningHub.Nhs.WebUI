using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// The Moodle Bridge Settings.
    /// </summary>
    public class MoodleBridgeAPIConfig
    {
        /// <summary>
        /// Gets or sets the base url for the Moodle service.
        /// </summary>
        public string BaseUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; } = null!;
    }
}
