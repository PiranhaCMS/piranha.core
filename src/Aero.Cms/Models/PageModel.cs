

namespace Aero.Cms.Models;

/// <summary>
/// Base class for basic content pages.
/// </summary>
[Serializable]
public class Page<T> : GenericPage<T> where T : Page<T> { }
