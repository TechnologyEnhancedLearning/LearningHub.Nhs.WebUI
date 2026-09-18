using LearningHub.Nhs.OpenApi.Services.Interface.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using LearningHub.Nhs.OpenApi.Models.ViewModels;



namespace LearningHub.Nhs.OpenApi.Services.Services
{
    /// <summary>
    /// The password manager service.
    /// </summary>
    public class PasswordManagerService : IPasswordManagerService
    {
        /// <summary>
        /// The password strategies.
        /// </summary>
        private readonly HashSet<PasswordStrategy> passwordStrategies;

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordManagerService"/> class.
        /// Creates a <see cref="PasswordManagerService" /> with default <see cref="PasswordStrategy"/>.
        /// </summary>
        public PasswordManagerService()
        {
            this.passwordStrategies = new HashSet<PasswordStrategy>
            {
                new PasswordAtLeastOneNumeric(),
                new PasswordAtLeastOneUpperCase(),
                new PasswordAtLeastOneLowerCase(),
                new PasswordAlphabeticNumericOrNonNumeric(5),
                new PasswordMinimumLength(8),
            };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PasswordManagerService"/> class.
        /// Creates a <see cref="PasswordManagerService"/> with a given collection of <see cref="PasswordStrategy"/>.
        /// </summary>
        /// <param name="passwordStrategies">
        /// The password strategies.
        /// </param>
        public PasswordManagerService(IEnumerable<PasswordStrategy> passwordStrategies)
        {
            this.passwordStrategies = new HashSet<PasswordStrategy>(passwordStrategies);
        }

        /// <inheritdoc/>
        public string Generate(int seed = 0)
        {
            var result = string.Empty;
            var rnd = new Random(seed);
            return Enumerable.Range(0, this.passwordStrategies.Count)
                .OrderBy(o => rnd.Next())
                .Aggregate(result, (current, i) => current + this.passwordStrategies.ElementAt(i)
                                                       .Generate());
        }

        /// <inheritdoc/>
        public bool Check(string password)
        {
            var result = true;

            for (var i = 0; i < this.passwordStrategies.Count; i++)
            {
                result = this.passwordStrategies.ElementAt(i).Check(password);
                if (!result)
                {
                    break;
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public int Compare(string password, string passwordToCompare)
        {
            return string.CompareOrdinal(password.ToLowerInvariant(), passwordToCompare.ToLowerInvariant());
        }

        /// <inheritdoc/>
        public string Base64MD5HashDigest(string szString)
        {
            var md5 = new MD5CryptoServiceProvider();
            var eEncoding = new ASCIIEncoding();
            var abHashDigest = md5.ComputeHash(eEncoding.GetBytes(szString));

            return Convert.ToBase64String(abHashDigest);
        }
    }
}
