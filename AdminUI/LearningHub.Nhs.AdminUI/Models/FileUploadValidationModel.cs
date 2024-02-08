namespace LearningHub.Nhs.AdminUI.Models
{
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Defines the <see cref="FileUploadValidationModel" />.
    /// </summary>
    public class FileUploadValidationModel
    {
        /// <summary>
        /// Gets or sets the Image.
        /// </summary>
        public IFormFile Image { get; set; }

        /// <summary>
        /// Gets or sets the Name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the Width.
        /// </summary>
        public int[] Width { get; set; }

        /// <summary>
        /// Gets or sets the Height.
        /// </summary>
        public int[] Height { get; set; }

        /// <summary>
        /// Gets or sets the FileSize.
        /// </summary>
        public int FileSize { get; set; }

        /// <summary>
        /// Gets or sets the Extension.
        /// </summary>
        public string[] Extension { get; set; }
    }
}
