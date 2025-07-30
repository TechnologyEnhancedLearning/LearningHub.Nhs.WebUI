using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.Shared.Interfaces.Http
{
    /// <summary>
    /// Marker interface for the IOpenAPIHttpClient API HttpClient.
    ///
    /// <para>
    /// Inherits from <see cref="IAPIHttpClient"/> to enable
    /// dependency injection of a specific implementation configured with
    /// a openapi-related API endpoint or settings.
    /// </para>
    ///
    /// <para>
    /// This interface is currently empty and used solely to differentiate
    /// implementations that connect to different endpoints via configuration.
    /// It may be extended in the future with user-specific functionality or properties.
    /// </para>
    /// </summary>
    public interface IOpenApiHttpClient : IAPIHttpClient
    {

    }
}
