namespace LearningHub.Nhs.OpenApi.Models.Configuration
{
    /// <summary>
    ///  DatabricksConfig 
    /// </summary>
    public class DatabricksConfig
    {
        /// <summary>
        /// Gets or sets the ResourceId for the databricks instance.
        /// </summary>
        public string ResourceId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the base url for the databricks instance.
        /// </summary>
        public string InstanceUrl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the warehouse id for databricks.
        /// </summary>
        public string WarehouseId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the job id for databricks.
        /// </summary>
        public string JobId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the tenant Id of the service pricncipl.
        /// </summary>
        public string TenantId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the client Id of the service pricncipl.
        /// </summary>
        public string ClientId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the client scret of the service pricncipl.
        /// </summary>
        public string clientSecret { get; set; } = null!;

        /// <summary>
        /// Gets or sets the endpoint to check user permission.
        /// </summary>
        public string UserPermissionEndpoint { get; set; } = null!;


        /// <summary>
        /// Gets or sets the endpoint for course completion record.
        /// </summary>
        public string CourseCompletionEndpoint { get; set; } = null!;

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        public string Token { get; set; } = null!;

    }
}
