using Aero.Cms.Manager.UI.Shared.Services;

namespace Aero.Cms.Manager.UI.Web.Services;

public class FormFactor : IFormFactor
{
    public string GetFormFactor()
    {
        return "Web";
    }

    public string GetPlatform()
    {
        return Environment.OSVersion.ToString();
    }
}
