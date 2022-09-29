/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license. See the LICENSE file for details.
 *
 * https://github.com/piranhacms/piranha.core
 *
 */

using Xunit;

namespace Piranha.Tests.Utils;

public class InternalId
{
    //corrected to GenerateInternalId
    [Fact]
    public void ToTitleCase() {
        Assert.Equal("MyTestValue", Piranha.Utils.GenerateInternalId("mY test vAlUE"));
    }
}
