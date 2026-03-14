using IdGen;

namespace Aero.Cms;

public static class Snowflake
{
    private static readonly IdGenerator _idGenerator = new(Random.Shared.Next(1022));
    private static readonly object _lock = new();

    public static string NewId()
    {
        lock (_lock)
        {
            return _idGenerator.CreateId().ToString();
        }
    }
}