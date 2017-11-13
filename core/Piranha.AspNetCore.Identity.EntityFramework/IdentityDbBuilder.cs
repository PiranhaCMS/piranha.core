using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Piranha.AspNetCore.Identity.EntityFramework
{
    public sealed class IdentityDbBuilder
    {
        /// <summary>
        /// Default constructor.
        /// </summary>
        public IdentityDbBuilder()
        {
            Migrate = true;
        }

        /// <summary>
        /// Gets/sets the connection string.
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Gets/sets if the database should be updated
        /// to latest migration automatically.
        /// </summary>
        public bool Migrate { get; set; }

        /// <summary>
        /// Gets/sets the optional Identity options.
        /// </summary>
        public IdentityOptions IdentityOptions { get; set; }

        /// <summary>
        /// Gets/sets the optional Cookie options
        /// </summary>
        public CookieAuthenticationOptions CookieAuthenticationOptions { get; set; }

        /// <summary>
        /// Gets/sets the initial users to create
        /// </summary>
        public IdentityAppUser[] Users { get; set; }

    }
}