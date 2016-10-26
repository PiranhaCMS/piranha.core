/*
 * Copyright (c) 2016 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Piranha.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class AssetController : Controller
    {
        #region Members
        /// <summary>
        /// The currently embedded resource types.
        /// </summary>
        private Dictionary<string, string> contentTypes = new Dictionary<string, string>() {
            { ".ico", "image/x-icon" },
            { ".png", "image/png" },
            { ".css", "text/css" },
            { ".js", "text/javascript" }
        };
        #endregion

        /// <summary>
        /// Gets the embedded asset with the given path.
        /// </summary>
        /// <param name="path">The path</param>
        [Route("manager/assets/{*path}")]
        public IActionResult GetAsset(string path) {
            var assembly = typeof(AssetController).GetTypeInfo().Assembly;
            var stream = assembly.GetManifestResourceStream("Piranha.Manager.assets." + path.Replace("/", "."));

            if (stream != null) {
                return new FileStreamResult(stream, GetContentType(path));
            }
            return NotFound();
        }

        /// <summary>
        /// Gets the content type for the asset.
        /// </summary>
        /// <param name="path">The asset path</param>
        /// <returns>The content type</returns>
        private string GetContentType(string path) {
            try {
                return contentTypes[path.Substring(path.LastIndexOf("."))];
            } catch { }
            return "text/plain";
        }
    }
}
