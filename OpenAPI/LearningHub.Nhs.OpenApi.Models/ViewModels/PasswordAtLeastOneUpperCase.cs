namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    /// <summary>
    /// The password at least one upper case.
    /// </summary>
    public class PasswordAtLeastOneUpperCase : PasswordStrategy
    {
        /// <summary>
        /// The valid chars.
        /// </summary>
        private const string ValidChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordAtLeastOneUpperCase"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        public PasswordAtLeastOneUpperCase(int length = 1)
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
            return Generator(this.Length, ValidChars);
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
            return passwordToCheck.IndexOfAny(ValidChars.ToCharArray()) != -1;
        }
    }
}
