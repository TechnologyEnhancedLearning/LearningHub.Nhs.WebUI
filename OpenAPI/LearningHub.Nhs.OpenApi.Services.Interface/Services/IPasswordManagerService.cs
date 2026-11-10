using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearningHub.Nhs.OpenApi.Services.Interface.Services
{
    /// <summary>
    /// The PasswordManagerService interface.
    /// </summary>
    public interface IPasswordManagerService : IComparer<string>
    {
        /// <summary>
        /// The generate.
        /// </summary>
        /// <param name="seed">
        /// The seed.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Generate(int seed = 0);

        /// <summary>
        /// The check.
        /// </summary>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Check(string password);

        /// <summary>
        /// The base 64 m d 5 hash digest.
        /// </summary>
        /// <param name="szString">
        /// The sz string.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Base64MD5HashDigest(string szString);
    }
}
