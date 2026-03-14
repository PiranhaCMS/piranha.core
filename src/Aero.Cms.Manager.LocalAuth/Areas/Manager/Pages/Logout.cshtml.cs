

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aero.Cms.Manager.LocalAuth.Areas.Manager.Pages
{
    /// <summary>
    /// View model for the logout page.
    /// </summary>
    public class LogoutModel : PageModel
    {
        private readonly ISecurity _service;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="service">The current security service</param>
        public LogoutModel(ISecurity service)
        {
            _service = service;
        }

        /// <summary>
        /// Handles the logout page.
        /// </summary>
        public async Task<IActionResult> OnGet()
        {
            await _service.SignOut(HttpContext);

            return new RedirectToPageResult("Login");
        }
    }
}