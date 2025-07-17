using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.WebUI.Shared.Configuration.ConfigurationSeperatingInterfaces
{
    public interface IFindwiseSettings
    {
        public int ResourceSearchPageSize { get; set; }

        /// <summary>
        /// Gets or sets the CatalogueSearchPageSize.
        /// </summary>
        public int CatalogueSearchPageSize { get; set; }

        /// <summary>
        /// Gets or sets the AllCatalogueSearchPageSize.
        /// </summary>
        public int AllCatalogueSearchPageSize { get; set; }
    }
}
