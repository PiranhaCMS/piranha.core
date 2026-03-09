

using Aero.Cms.Models;
using Xunit;

namespace Aero.Cms.Tests.Utils;

public class DeepClone
{
    [Fact]
    public void WithBoolean()
    {
        bool parameter = true;

        bool result = Aero.Cms.Utils.DeepClone(parameter);
        parameter = false;

        // Result should be true and changing 'parameter' should not affect 'result'.
        Assert.True(result);
        Assert.NotEqual(parameter, result);
    }

    [Fact]
    public void WithAliasNull()
    {
        Alias alias = null;

        Alias result = Aero.Cms.Utils.DeepClone(alias);
        alias = new Alias();

        // Result should be null and not referentially equal to 'parameter'.
        Assert.Null(result);
        Assert.NotEqual(alias, result);
    }

    [Fact]
    public void WithAliasObject()
    {
        Alias alias = new Alias();

        Alias result = Aero.Cms.Utils.DeepClone<Alias>(alias);
        alias.Id = Snowflake.NewId();

        // Result should not be null true and not referentially equal to 'parameter'.
        Assert.NotNull(result);
        Assert.NotEqual(alias, result);
    }
}
