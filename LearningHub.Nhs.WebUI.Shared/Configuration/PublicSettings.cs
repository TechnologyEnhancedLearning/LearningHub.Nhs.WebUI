using LearningHub.Nhs.WebUI.Shared.Configuration.ConfigurationSeperatingInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.WebUI.Shared.Configuration
{
    public class PublicSettings : IPublicSettings
    {
        public string LearningHubApiUrl { get; set; }
        public int ResourceSearchPageSize { get; set; }
        public int CatalogueSearchPageSize { get; set; }
        public int AllCatalogueSearchPageSize { get; set; }
        public IFindwiseSettings FindwiseSettings { get; set; }
    }
}
