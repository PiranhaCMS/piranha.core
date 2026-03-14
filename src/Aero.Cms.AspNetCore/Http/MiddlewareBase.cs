

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Aero.Cms.AspNetCore.Services;

namespace Aero.Cms.AspNetCore.Http;

/// <summary>
/// Base class for middleware.
/// </summary>
public abstract class MiddlewareBase
{
    /// <summary>
    /// The next middleware in the pipeline.
    /// </summary>
    protected readonly RequestDelegate _next;

    /// <summary>
    /// The optional logger.
    /// </summary>
    protected ILogger _logger;

    /// <summary>
    /// Creates a new middleware instance.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    public MiddlewareBase(RequestDelegate next)
    {
        _next = next;
    }

    /// <summary>
    /// Creates a new middleware instance.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline</param>
    /// <param name="factory">The logger factory</param>
    public MiddlewareBase(RequestDelegate next, ILoggerFactory factory) : this(next)
    {
        if (factory != null)
        {
            _logger = factory.CreateLogger(this.GetType().FullName);
        }
    }

    /// <summary>
    /// Invokes the middleware.
    /// </summary>
    /// <param name="context">The current http context</param>
    /// <param name="api">The current api</param>
    /// <param name="service">The application service</param>
    /// <returns>An async task</returns>
    public abstract Task Invoke(HttpContext context, IApi api, IApplicationService service);

    /// <summary>
    /// Checks if the request has already been handled by another
    /// Aero.Cms middleware.
    /// </summary>
    /// <param name="context">The current http context</param>
    /// <returns>If the request has already been handled</returns>
    protected bool IsHandled(HttpContext context)
    {
        var values = context.Request.Query["Aero_handled"];
        if (values.Count > 0)
        {
            return values[0] == "true";
        }
        return false;
    }

    /// <summary>
    /// Checks if the request wants a draft.
    /// </summary>
    /// <param name="context">The current http context</param>
    /// <returns>If the request is for a draft</returns>
    protected bool IsDraft(HttpContext context)
    {
        var values = context.Request.Query["draft"];
        if (values.Count > 0)
        {
            return values[0] == "true";
        }
        return false;
    }

    /// <summary>
    /// Checks if this is a manager request.
    /// </summary>
    /// <param name="url">The url</param>
    /// <returns>If the given url is for the manager application</returns>
    protected bool IsManagerRequest(string url)
    {
        if (string.IsNullOrEmpty(url))
        {
            return false;
        }

        if (url.StartsWith("/manager/") || url == "/manager")
        {
            return true;
        }
        return false;
    }
}

