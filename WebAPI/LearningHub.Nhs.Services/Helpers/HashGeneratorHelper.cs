// <copyright file="HashGeneratorHelper.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.Services.Helpers
{
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// A class containing helper functions for generating hash code.
    /// </summary>
    public static class HashGeneratorHelper
    {
        /// <summary>
        /// Generate hash code.
        /// </summary>
        /// <param name="input">input string.</param>
        /// <returns>string.</returns>
        public static string SHA256ToString(string input)
        {
            using (var alg = SHA256.Create())
            {
                return alg.ComputeHash(Encoding.UTF8.GetBytes(input)).Aggregate(new StringBuilder(), (sb, x) => sb.Append(x.ToString("x2"))).ToString();
            }
        }
    }
}
