

using Shouldly;

namespace Aero.Cms.Tests;

public class Taxonomies
{
    [Fact]
    public void StringToTaxonomy() {
        Models.Taxonomy t = "Test";

        Assert.NotNull(t);
        string.IsNullOrEmpty(t.Id).ShouldBeTrue();
        Assert.Equal("Test", t.Title);
        Assert.Null(t.Slug);
    }
}
