namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    /// <summary>
    /// The password minimum length.
    /// </summary>
    public class PasswordMinimumLength : PasswordStrategy
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordMinimumLength"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        public PasswordMinimumLength(int length = 1)
        {
            this.Length = length;
        }

        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        protected sealed override int Length { get; set; }

        /// <summary>
        /// The generate.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string Generate()
        {
            return string.Empty;
        }

        /// <summary>
        /// The check.
        /// </summary>
        /// <param name="passwordToCheck">
        /// The password to check.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool Check(string passwordToCheck)
        {
            return passwordToCheck.Length >= this.Length;
        }
    }
}