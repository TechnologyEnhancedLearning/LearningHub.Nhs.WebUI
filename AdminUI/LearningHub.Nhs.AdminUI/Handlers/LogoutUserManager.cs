// <copyright file="LogoutUserManager.cs" company="NHS England">
// Copyright (c) NHS England.
// </copyright>

namespace LearningHub.Nhs.AdminUI.Handlers
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Defines the <see cref="LogoutUserManager" />.
    /// </summary>
    public class LogoutUserManager
    {
        /// <summary>
        /// Defines the _user.
        /// </summary>
        private List<User> user = new List<User>();

        /// <summary>
        /// The Add.
        /// </summary>
        /// <param name="sub">The sub<see cref="string"/>.</param>
        /// <param name="sid">The sid<see cref="string"/>.</param>
        public void Add(string sub, string sid)
        {
            this.user.Add(new User { Sub = sub, Sid = sid });
        }

        /// <summary>
        /// The IsLoggedOut.
        /// </summary>
        /// <param name="sub">The sub<see cref="string"/>.</param>
        /// <param name="sid">The sid<see cref="string"/>.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        public bool IsLoggedOut(string sub, string sid)
        {
            var matches = this.user.Any(s => s.IsMatch(sub, sid));
            return matches;
        }

        /// <summary>
        /// Defines the <see cref="User" />.
        /// </summary>
        private class User
        {
            /// <summary>
            /// Gets or sets the Sid.
            /// </summary>
            public string Sid { get; set; }

            /// <summary>
            /// Gets or sets the Sub.
            /// </summary>
            public string Sub { get; set; }

            /// <summary>
            /// The IsMatch.
            /// </summary>
            /// <param name="sub">The sub<see cref="string"/>.</param>
            /// <param name="sid">The sid<see cref="string"/>.</param>
            /// <returns>The <see cref="bool"/>.</returns>
            public bool IsMatch(string sub, string sid)
            {
                return (this.Sid == sid && this.Sub == sub) ||
                       (this.Sid == sid && this.Sub == null) ||
                       (this.Sid == null && this.Sub == sub);
            }
        }
    }
}
