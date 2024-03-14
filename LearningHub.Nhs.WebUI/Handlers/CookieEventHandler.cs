namespace LearningHub.Nhs.WebUI.Handlers
{
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.Cookies;

    /// <summary>
    /// Defines the <see cref="CookieEventHandler" />.
    /// </summary>
    public class CookieEventHandler : CookieAuthenticationEvents
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CookieEventHandler"/> class.
        /// </summary>
        /// <param name="logoutSessions">Logout sessions.</param>
        public CookieEventHandler(LogoutUserManager logoutSessions)
        {
            this.LogoutUsers = logoutSessions;
        }

        /// <summary>
        /// Gets the LogoutUsers.
        /// </summary>
        public LogoutUserManager LogoutUsers { get; }

        /// <summary>
        /// The ValidatePrincipal.
        /// </summary>
        /// <param name="context">The context<see cref="CookieValidatePrincipalContext"/>.</param>
        /// <returns>The <see cref="Task"/>.</returns>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            if (context.Principal.Identity.IsAuthenticated)
            {
                var sub = context.Principal.FindFirst("sub")?.Value;
                var sid = context.Principal.FindFirst("sid")?.Value;

                if (this.LogoutUsers.IsLoggedOut(sub, sid))
                {
                    context.RejectPrincipal();
                    await context.HttpContext.SignOutAsync();
                }
            }
        }
    }
}
