namespace LearningHub.Nhs.OpenApi.Services.Helpers
{
    /// <summary>
    /// TextCasingHelper.
    /// </summary>
    public class TextCasingHelper
    {
        /// <summary>
        /// Returns sentence case of input string.
        /// </summary>
        /// <param name="input">input.</param>
        /// <returns>A sentence case string corresponding to the input string.</returns>
        public static string ConvertToSentenceCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            input = input.ToLower();
            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }
}
