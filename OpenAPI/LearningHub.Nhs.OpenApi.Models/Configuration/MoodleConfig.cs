using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    /// The Moodle Settings.
    /// </summary>
    public class MoodleConfig
    {
        /// <summary>
        /// Gets or sets the base url for the Moodle service.
        /// </summary>
        public string APIBaseUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Web service Rest Format.
        /// </summary>
        public string APIWSRestFormat { get; set; } = null!;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string APIWSToken { get; set; } = null!;
    }
}
