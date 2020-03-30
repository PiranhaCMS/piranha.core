/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using Xunit;

namespace Piranha.Tests.Utils
{
    public class InternalId
    {
        [Fact]
        public void ToTitleCase() {
            Assert.Equal("MyTestValue", Piranha.Utils.GenerateInteralId("mY test vAlUE"));
        }
    }
}
