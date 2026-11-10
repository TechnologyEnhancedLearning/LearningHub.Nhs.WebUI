using LearningHub.Nhs.Models.Organisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// Provides access to the NHS Organisation Data Service API.
    /// </summary>
    public interface IOdsOrganisationClient
    {
        /// <summary>
        /// Gets an organisation from ODS by ODS code.
        /// </summary>
        /// <param name="odsCode">The ODS code.</param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The ODS organisation, or null when the code
        /// does not exist.
        /// </returns>
        Task<OdsOrganisationRecord?> GetByOdsCodeAsync(string odsCode, CancellationToken cancellationToken = default);
    }
}
