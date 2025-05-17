using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ContentRus.HostedSite.Models;

[PageType(Title = "Standard archive", IsArchive = true)]
public class StandardArchive : Page<StandardArchive>
{
    /// <summary>
    /// The currently loaded post archive.
    /// </summary>
    public PostArchive<PostInfo> Archive { get; set; }
}
