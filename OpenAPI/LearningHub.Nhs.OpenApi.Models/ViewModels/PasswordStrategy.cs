namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    using System;
    using System.Text;

    /// <summary>
    /// The password strategy.
    /// </summary>
    public abstract class PasswordStrategy
    {
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        protected abstract int Length { get; set; }

        /// <summary>
        /// The generate.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public abstract string Generate();

        /// <summary>
        /// The check.
        /// </summary>
        /// <param name="passwordToCheck">
        /// The password to check.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public abstract bool Check(string passwordToCheck);

        /// <summary>
        /// The generator.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        /// <param name="validChars">
        /// The valid chars.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        protected static string Generator(int length, string validChars)
        {
            var result = new StringBuilder();
            var rnd = new Random();
            while (result.Length < length)
            {
                result.Append(validChars[rnd.Next(validChars.Length)]);
            }

            return result.ToString();
        }
    }
}