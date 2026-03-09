

using Xunit;

namespace Aero.Cms.Tests.Utils;

public class ETag
{
    [Fact]
    public void UniqueById() {
        var date = DateTime.Now;

        var etag1 = Aero.Cms.Utils.GenerateETag(Snowflake.NewId().ToString(), date);
        var etag2 = Aero.Cms.Utils.GenerateETag(Snowflake.NewId().ToString(), date);

        Assert.NotEqual(etag1, etag2);
    }

    [Fact]
    public void UniqueByDate() {
        var id = Snowflake.NewId().ToString();
        var date = DateTime.Now;

        var etag1 = Aero.Cms.Utils.GenerateETag(id, date);
        var etag2 = Aero.Cms.Utils.GenerateETag(id, date.AddDays(-1));

        Assert.NotEqual(etag1, etag2);
    }

    [Fact]
    public void EqualTags() {
        var id = Snowflake.NewId().ToString();
        var date = DateTime.Now;

        var etag1 = Aero.Cms.Utils.GenerateETag(id, date);
        var etag2 = Aero.Cms.Utils.GenerateETag(id, date);

        Assert.Equal(etag1, etag2);
    }
}
