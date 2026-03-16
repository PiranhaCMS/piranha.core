

using System.Text.Json;

namespace Aero.Cms.Data.Data;

public abstract class ContentRevisionBase : Entity
{
    /// <summary>
    /// Gets/sets the data of the revision serialized
    /// as JSON.
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    /// Gets/sets the created date.
    /// </summary>
    public DateTime Created { get; set; }

    /// <summary>
    /// Gets the revision data deserialized as the
    /// specified type.
    /// </summary>
    /// <typeparam name="T">The type</typeparam>
    /// <returns>The deserialized revision data</returns>
    public T GetData<T>()
    {
        if (!string.IsNullOrEmpty(Data))
            return JsonSerializer.Deserialize<T>(Data);
        return default(T);
    }
}
