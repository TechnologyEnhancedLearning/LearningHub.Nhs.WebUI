
namespace LearningHub.Nhs.OpenApi.Models.ViewModels
{
    /// <summary>
    /// The password alphabetic numeric or non numeric.
    /// </summary>
    public class PasswordAlphabeticNumericOrNonNumeric : PasswordStrategy
    {
        /// <summary>
        /// The valid chars.
        /// </summary>
        private const string ValidChars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~!@#$%^&*()-+=?/<>|[]{}_:;.,\\`.";

        /// <summary>
        /// The generate chars.
        /// </summary>
        private const string GenerateChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordAlphabeticNumericOrNonNumeric"/> class.
        /// </summary>
        /// <param name="length">
        /// The length.
        /// </param>
        public PasswordAlphabeticNumericOrNonNumeric(int length = 1)
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
            return Generator(this.Length, GenerateChars);
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
            var returnVal = true;
            foreach (var chr in passwordToCheck)
            {
                if (ValidChars.IndexOf(chr) == -1)
                {
                    returnVal = false;
                    break;
                }
            }

            return returnVal;
        }
    }
}