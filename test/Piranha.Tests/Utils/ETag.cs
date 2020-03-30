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

namespace Piranha.Tests.Utils
{
    public class ETag
    {
        [Fact]
        public void UniqueById() {
            var date = DateTime.Now;

            var etag1 = Piranha.Utils.GenerateETag(Guid.NewGuid().ToString(), date);
            var etag2 = Piranha.Utils.GenerateETag(Guid.NewGuid().ToString(), date);

            Assert.NotEqual(etag1, etag2);
        }

        [Fact]
        public void UniqueByDate() {
            var id = Guid.NewGuid().ToString();
            var date = DateTime.Now;

            var etag1 = Piranha.Utils.GenerateETag(id, date);
            var etag2 = Piranha.Utils.GenerateETag(id, date.AddDays(-1));

            Assert.NotEqual(etag1, etag2);
        }

        [Fact]
        public void EqualTags() {
            var id = Guid.NewGuid().ToString();
            var date = DateTime.Now;

            var etag1 = Piranha.Utils.GenerateETag(id, date);
            var etag2 = Piranha.Utils.GenerateETag(id, date);

            Assert.Equal(etag1, etag2);
        }
    }
}
