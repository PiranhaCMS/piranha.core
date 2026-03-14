

namespace Aero.Cms.Manager.Models;

public class AsyncResult<T>
{
    /// <summary>
    /// Gets/sets the result body.
    /// </summary>
    public T Body { get; set; }
}

/// <summary>
/// Result model.
/// </summary>
public class AsyncResult
{
    /// <summary>
    /// Gets/sets the status message from the last operation.
    /// </summary>
    public StatusMessage Status { get; set; }
}
