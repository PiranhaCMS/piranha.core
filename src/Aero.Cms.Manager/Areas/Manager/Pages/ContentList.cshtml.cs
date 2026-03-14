/*
 * Copyright (c) 2019 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/Aerocms/Aero.core
 *
 */

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Aero.Cms.Manager.Models
{
    [Authorize(Policy = Permission.Content)]
    public class ContentListViewModel : PageModel
    {
    }
}