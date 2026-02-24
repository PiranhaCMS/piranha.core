using IdGen;

namespace Piranha;

public static class Snowflake
{
    // todo - move to aero.core
    public static string NewId()
    {
        var idgen = new IdGenerator(Random.Shared.Next(1022));
        var id = idgen.CreateId().ToString();
        return id;
    }
}