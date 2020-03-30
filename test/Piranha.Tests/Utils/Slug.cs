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
    public class Slug
    {
        [Fact]
        public void ToLowerCase() {
            Assert.Equal("mycamelcasestring", Piranha.Utils.GenerateSlug("MyCamelCaseString"));
        }

        [Fact]
        public void Trim() {
            Assert.Equal("trimmed", Piranha.Utils.GenerateSlug(" trimmed  "));
        }

        [Fact]
        public void ReplaceWhitespace() {
            Assert.Equal("no-whitespaces", Piranha.Utils.GenerateSlug("no whitespaces"));
        }

        [Fact]
        public void ReplaceDoubleDashes() {
            Assert.Equal("no-whitespaces", Piranha.Utils.GenerateSlug("no - whitespaces"));
            Assert.Equal("no-whitespaces", Piranha.Utils.GenerateSlug("no & whitespaces"));
        }

        [Fact]
        public void TrimDashes() {
            Assert.Equal("trimmed", Piranha.Utils.GenerateSlug("-trimmed-"));
        }

        [Fact]
        public void RemoveSwedishCharacters() {
            Assert.Equal("aaoaao", Piranha.Utils.GenerateSlug("åäöÅÄÖ"));
        }

        [Fact]
        public void RemoveHyphenCharacters() {
            Assert.Equal("aaooeeiiaaooeeii", Piranha.Utils.GenerateSlug("áàóòéèíìÁÀÓÒÉÈÍÌ"));
        }

        [Fact]
        public void RemoveSlashesForNonHierarchical() {
            Assert.Equal("no-more-dashes", Piranha.Utils.GenerateSlug("no/more / dashes", false));
        }
    }
}
