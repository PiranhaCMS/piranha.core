/*
 * Copyright (c) 2018 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * https://github.com/piranhacms/piranha.core
 * 
 */

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Piranha.Areas.Manager.Controllers
{
    public abstract class MessageControllerBase : Controller
    {
		public override void OnActionExecuting(ActionExecutingContext context) {
            if (TempData.ContainsKey("MessageCss")) {
                ViewBag.MessageCss = TempData["MessageCss"];
                TempData.Remove("MessageCss");
            }
            if (TempData.ContainsKey("Message")) {
                ViewBag.Message = TempData["Message"];
                TempData.Remove("Message");
            }
		}

		/// <summary>
		/// Adds a success message to the current view.
		/// </summary>
		/// <param name="msg">The message</param>
		/// <param name="persist">If the message should be persisted in TempData</param>
		protected void SuccessMessage(string msg, bool persist = true) {
			msg = "<b>Success:</b> " + msg;

			ViewBag.MessageCss = "alert alert-success";
			ViewBag.Message = msg;
			if (persist) {
				TempData["MessageCss"] = "alert alert-success";
				TempData["Message"] = msg;
			}
        }

		/// <summary>
		/// Adds an error message to the current view.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="persist">If the message should be persisted in TempData</param>
		protected void ErrorMessage(string msg, bool persist = true) {
			msg = "<b>Error:</b> " + msg;

			ViewBag.MessageCss = "alert alert-danger";
			ViewBag.Message = msg;
			if (persist) {
				TempData["MessageCss"] = "alert alert-danger";
				TempData["Message"] = msg;
			}
        }

		/// <summary>
		/// Adds an information message to the current view.
		/// </summary>
		/// <param name="msg"></param>
		/// <param name="persist">If the message should be persisted in TempData</param>
		protected void InformationMessage(string msg, bool persist = true) {
			msg = "<b>Information:</b> " + msg;

			ViewBag.MessageCss = "alert alert-info";
			ViewBag.Message = msg;
			if (persist) {
				TempData["MessageCss"] = "alert alert-info";
				TempData["Message"] = msg;
			}
        }    
    }
}