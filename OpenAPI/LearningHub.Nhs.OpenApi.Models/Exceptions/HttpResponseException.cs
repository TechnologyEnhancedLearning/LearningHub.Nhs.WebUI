namespace LearningHub.Nhs.OpenApi.Models.Exceptions
{
    using System;
    using System.Net;

    /// <summary>
    /// Should be used when a failure response should be sent.
    /// </summary>
    public class HttpResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException"/> class.
        /// </summary>
        /// <param name="message"><see cref="ResponseBody"/>.</param>
        /// <param name="statusCode"><see cref="StatusCode"/>.</param>
        public HttpResponseException(string? message, HttpStatusCode statusCode)
            : base(message)
        {
            this.ResponseBody = message;
            this.StatusCode = statusCode;
        }

        /// <summary>
        /// Gets or sets StatusCode.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Gets or sets ResponseBody.
        /// </summary>
        public object? ResponseBody { get; set; }
    }
}
