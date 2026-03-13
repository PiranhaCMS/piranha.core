

namespace Aero.Cms.Tests.Utils;

public class InternalId
{
    //corrected to GenerateInternalId
    [Fact]
    public void ToTitleCase() {
        Assert.Equal("MyTestValue", Aero.Cms.Utils.GenerateInternalId("mY test vAlUE"));
    }
}
