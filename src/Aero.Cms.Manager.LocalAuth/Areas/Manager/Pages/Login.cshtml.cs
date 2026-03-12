

using System.ComponentModel.DataAnnotations;
using Aero.Manager;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Aero.Cms.Manager.LocalAuth.Areas.Manager.Pages;

/// <summary>
/// View model for the login page.
/// </summary>
public class LoginModel : PageModel
{
    private readonly ISecurity _service;
    private readonly ManagerLocalizer _localizer;
    private readonly ILogger<LoginModel> log;

    /// <summary>
    /// Default constructor.
    /// </summary>
    /// <param name="service">The current security service</param>
    /// <param name="localizer">The manager localizer</param>
    public LoginModel(ISecurity service, ManagerLocalizer localizer, ILogger<LoginModel> log)
    {
        _service = service;
        _localizer = localizer;
        this.log = log;
    }

    /// <summary>
    /// Gets/sets the model for binding form data.
    /// </summary>
    /// <value></value>
    [BindProperty]
    public InputModel Input { get; set; }

    /// <summary>
    /// Gets/sets the optional return url after successful
    /// authorization.
    /// </summary>
    public string ReturnUrl { get; set; }

    /// <summary>
    /// Gets/sets the possible error message to be returned
    /// after failed authorization.
    /// </summary>
    [TempData]
    public string ErrorMessage { get; set; }

    /// <summary>
    /// Model for form data.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        /// Gets/sets the user name.
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets/sets the password.
        /// </summary>
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    /// <summary>
    /// Gets the login page.
    /// </summary>
    /// <param name="returnUrl">The optional return url</param>
    public void OnGet(string returnUrl = null)
    {
        log.LogInformation("Login page visited.");
        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            ModelState.AddModelError(string.Empty, ErrorMessage);
        }

        ReturnUrl = returnUrl;
    }

    /// <summary>
    /// Handles authorization after a post.
    /// </summary>
    /// <param name="returnUrl">The optional return url</param>
    public async Task<IActionResult> OnPostAsync(string returnUrl = null)
    {
        var authed = HttpContext.User.Identity?.IsAuthenticated ?? false;

        if(authed)
            await _service.SignOut(HttpContext);

        var res = await _service.SignIn(HttpContext, Input.Username, Input.Password);
        if (!ModelState.IsValid || res != LoginResult.Succeeded)
        {
            ModelState.Clear();
            ModelState.AddModelError(string.Empty, _localizer.General["Username and/or password are incorrect."].Value);
            return Page();
        }
        
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return LocalRedirect($"~/manager/login/auth?returnUrl={ returnUrl }");
        }
        return LocalRedirect("~/manager/login/auth");
    }
}