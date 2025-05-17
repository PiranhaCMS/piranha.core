using Piranha.AttributeBuilder;
using Piranha.Models;

namespace ContentRus.Manager.Models;

[PostType(Title = "Standard post")]
public class StandardPost  : Post<StandardPost>
{
    /// <summary>
    /// Gets/sets the available comments if these
    /// have been loaded from the database.
    /// </summary>
    public IEnumerable<Comment> Comments { get; set; } = new List<Comment>();
}
