

using Aero.Cms;
using Aero.AspNetCore;

/// <summary>
/// Extension class for adding TinyMCE to the web application.
/// </summary>
public static class TinyMCEStartupExtensions
{
    /// <summary>
    /// Adds the Tiny MCE editor module if simple startup is used.
    /// </summary>
    /// <param name="serviceBuilder">The service builder</param>
    /// <returns>The updated builder</returns>
    public static AeroServiceBuilder UseTinyMCE(this AeroServiceBuilder serviceBuilder)
    {
        serviceBuilder.Services.AddAeroTinyMCE();

        return serviceBuilder;
    }

    /// <summary>
    /// Uses the Tiny MCE editor module if simple startup is used.
    /// </summary>
    /// <param name="applicationBuilder">The application builder</param>
    /// <returns>The updated builder</returns>
    public static AeroApplicationBuilder UseTinyMCE(this AeroApplicationBuilder applicationBuilder)
    {
        applicationBuilder.Builder.UseAeroTinyMCE();

        return applicationBuilder;
    }
}
