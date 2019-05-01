/*
 * Copyright (c) 2017-2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class StartController : Controller
    {
        private readonly IAuthorizationService _auth;

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="auth">The authorization service</param>
        public StartController(IAuthorizationService auth)
        {
            _auth = auth;
        }

        /// <summary>
        /// Redirects to the current startpage of the
        /// manager area.
        /// </summary>
        [Route("manager")]
        public async Task<IActionResult> Index()
        {
            var menu = await Piranha.Manager.Menu.GetForUser(User, _auth);

            if (menu.Count == 0)
            {
                // User doesn't have access to anything in the manager,
                // redirect to logout
                return RedirectToAction("Logout", "Account");
            }

            // Redirect to the first item the user has access to.
            var firstItem = menu.First().Items.First();

            return RedirectToAction(firstItem.Action, firstItem.Controller);
        }
    }
}
