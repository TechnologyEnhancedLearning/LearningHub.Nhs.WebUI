using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.WebUI.Shared.Configuration.ConfigurationSeperatingInterfaces
{
    public interface IPublicSettings
    {
        /// <summary>
        /// Gets or sets the LearningHubApiUrl.
        /// </summary>
        public string LearningHubApiUrl { get; set; }

        public IFindwiseSettings FindwiseSettings { get; set; }
    }
}
