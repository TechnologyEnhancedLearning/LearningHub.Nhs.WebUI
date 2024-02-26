namespace LearningHub.Nhs.Services.Helpers
{
    /// <summary>
    /// The querystring helper.
    /// </summary>
    public class BinaryFormatterHelper
    {
        /// <summary>
        /// Encode to base64.
        /// </summary>
        /// <param name="json">json string.</param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string Base64EncodeObject(string json)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(json);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
