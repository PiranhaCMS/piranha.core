/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Piranha.Manager.Models;

namespace Piranha.Manager.Controllers
{
    /// <summary>
    /// Api controller for getting user permissions.
    /// </summary>
    [Area("Manager")]
    [Route("manager/api/permissions")]
    [Authorize(Policy = Permission.Admin)]
    [ApiController]
    public class PermissionApiController : Controller
    {
        private readonly IAuthorizationService _auth;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PermissionApiController(IAuthorizationService auth)
        {
            _auth = auth;
        }

        [HttpGet]
        [Authorize(Policy = Permission.Admin)]
        public async Task<PermissionModel> Get()
        {
            var model = new PermissionModel();

            // Alias permissions
            model.Aliases.Edit = (await _auth.AuthorizeAsync(User, Permission.AliasesEdit)).Succeeded;
            model.Aliases.Delete = (await _auth.AuthorizeAsync(User, Permission.AliasesDelete)).Succeeded;

            // Comment permissions
            model.Comments.Approve = (await _auth.AuthorizeAsync(User, Permission.CommentsApprove)).Succeeded;
            model.Comments.Delete = (await _auth.AuthorizeAsync(User, Permission.CommentsDelete)).Succeeded;

            // Media permissions
            model.Media.Add = (await _auth.AuthorizeAsync(User, Permission.MediaAdd)).Succeeded;
            model.Media.AddFolder = (await _auth.AuthorizeAsync(User, Permission.MediaAddFolder)).Succeeded;
            model.Media.Delete = (await _auth.AuthorizeAsync(User, Permission.MediaDelete)).Succeeded;
            model.Media.DeleteFolder = (await _auth.AuthorizeAsync(User, Permission.MediaDeleteFolder)).Succeeded;
            model.Media.Edit = (await _auth.AuthorizeAsync(User, Permission.MediaEdit)).Succeeded;

            // Page permissions
            model.Pages.Add = (await _auth.AuthorizeAsync(User, Permission.PagesAdd)).Succeeded;
            model.Pages.Delete = (await _auth.AuthorizeAsync(User, Permission.PagesDelete)).Succeeded;
            model.Pages.Edit = (await _auth.AuthorizeAsync(User, Permission.PagesEdit)).Succeeded;
            model.Pages.Preview = (await _auth.AuthorizeAsync(User, Security.Permission.PagePreview)).Succeeded;
            model.Pages.Publish = (await _auth.AuthorizeAsync(User, Permission.PagesPublish)).Succeeded;
            model.Pages.Save = (await _auth.AuthorizeAsync(User, Permission.PagesSave)).Succeeded;

            // Post permissions
            model.Posts.Add = (await _auth.AuthorizeAsync(User, Permission.PostsAdd)).Succeeded;
            model.Posts.Delete = (await _auth.AuthorizeAsync(User, Permission.PostsDelete)).Succeeded;
            model.Posts.Edit = (await _auth.AuthorizeAsync(User, Permission.PostsEdit)).Succeeded;
            model.Posts.Preview = (await _auth.AuthorizeAsync(User, Security.Permission.PostPreview)).Succeeded;
            model.Posts.Publish = (await _auth.AuthorizeAsync(User, Permission.PostsPublish)).Succeeded;
            model.Posts.Save = (await _auth.AuthorizeAsync(User, Permission.PostsSave)).Succeeded;

            // Site permissions
            model.Sites.Add = (await _auth.AuthorizeAsync(User, Permission.SitesAdd)).Succeeded;
            model.Sites.Delete = (await _auth.AuthorizeAsync(User, Permission.SitesDelete)).Succeeded;
            model.Sites.Edit = (await _auth.AuthorizeAsync(User, Permission.SitesEdit)).Succeeded;
            model.Sites.Save = (await _auth.AuthorizeAsync(User, Permission.SitesSave)).Succeeded;

            return model;
        }
    }
}