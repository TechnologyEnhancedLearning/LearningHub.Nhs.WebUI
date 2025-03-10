namespace LearningHub.Nhs.WebUI.AutomatedUiTests
{
    using System;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using LearningHub.Nhs.WebUI.AutomatedUiTests.TestHelpers;
    using LearningHub.Nhs.WebUI.Startup;
    using Microsoft.AspNetCore;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Hosting.Server.Features;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Configuration.Json;
    using Serilog;

    /// <summary>
    /// SeleniumServerFactory.
    /// </summary>
    public class SeleniumServerFactory
    {
        /// <summary>
        /// Root Uri.
        /// </summary>
        public string RootUri;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeleniumServerFactory"/> class.
        /// </summary>
        public SeleniumServerFactory()
        {
            IConfiguration configuration = ConfigurationHelper.GetConfiguration();
            this.RootUri = configuration["LearningHubWebUiUrl"];
        }
    }
}
