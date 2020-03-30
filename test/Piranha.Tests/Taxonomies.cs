/*
 * Copyright (c) .NET Foundation and Contributors
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 *
 * http://github.com/piranhacms/piranha
 *
 */

using System;
using Xunit;

namespace Piranha.Tests
{
    public class Taxonomies
    {
        [Fact]
        public void StringToTaxonomy() {
            Models.Taxonomy t = "Test";

            Assert.NotNull(t);
            Assert.Equal(Guid.Empty, t.Id);
            Assert.Equal("Test", t.Title);
            Assert.Null(t.Slug);
        }
    }
}