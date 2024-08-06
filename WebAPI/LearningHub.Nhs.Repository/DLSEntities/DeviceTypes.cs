namespace LearningHub.Nhs.Api.DLSEntities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The DLSDeviceType class.
    /// </summary>
    public class DeviceTypes
    {
        /// <summary>
        /// Gets or sets the id for the DLSDeviceType.
        /// </summary>
        [Key]
        public int DeviceTypeId { get; set; }

        /// <summary>
        /// Gets or sets the deviceType for the DLSDeviceType.
        /// </summary>
        public string DeviceType { get; set; }
    }
}
