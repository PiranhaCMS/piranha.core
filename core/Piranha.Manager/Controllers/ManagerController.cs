/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Base mvc controller for the manager interface.
    /// </summary>
    [Area("Manager")]
    public abstract class ManagerController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (TempData.ContainsKey("MessageCss"))
            {
                ViewBag.MessageCss = TempData["MessageCss"];
                TempData.Remove("MessageCss");
            }
            if (TempData.ContainsKey("Message"))
            {
                ViewBag.Message = TempData["Message"];
                TempData.Remove("Message");
            }
        }

        /// <summary>
        /// Adds a success message to the current view.
        /// </summary>
        /// <param name="msg">The message</param>
        /// <param name="persist">If the message should be persisted in TempData</param>
        protected void SuccessMessage(string msg, bool persist = true)
        {
            msg = "<b>Success:</b> " + msg;

            ViewBag.MessageCss = Models.StatusMessage.Success;
            ViewBag.Message = msg;
            if (persist)
            {
                TempData["MessageCss"] = Models.StatusMessage.Success;
                TempData["Message"] = msg;
            }
        }

        /// <summary>
        /// Adds an error message to the current view.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="persist">If the message should be persisted in TempData</param>
        protected void ErrorMessage(string msg, bool persist = true)
        {
            msg = "<b>Error:</b> " + msg;

            ViewBag.MessageCss = Models.StatusMessage.Error;
            ViewBag.Message = msg;
            if (persist)
            {
                TempData["MessageCss"] = Models.StatusMessage.Error;
                TempData["Message"] = msg;
            }
        }

        /// <summary>
        /// Adds an information message to the current view.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="persist">If the message should be persisted in TempData</param>
        protected void InformationMessage(string msg, bool persist = true)
        {
            msg = "<b>Information:</b> " + msg;

            ViewBag.MessageCss = Models.StatusMessage.Information;
            ViewBag.Message = msg;
            if (persist)
            {
                TempData["MessageCss"] = Models.StatusMessage.Information;
                TempData["Message"] = msg;
            }
        }
    }
}