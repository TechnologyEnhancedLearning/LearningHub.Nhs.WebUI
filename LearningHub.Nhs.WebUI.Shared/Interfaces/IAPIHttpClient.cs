using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.WebUI.Shared.Interfaces
{
    public interface IAPIHttpClient
    {
        /// <summary>
        /// The get client.
        /// </summary>
        /// <returns>The <see cref="Task"/>.</returns>
        Task<HttpClient> GetClientAsync();

        public string ApiUrl { get; }//qqqq
    }
}
